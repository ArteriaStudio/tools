
/*　データベースを生成　*/
CREATE DATABASE sbook WITH OWNER = bkowner;


/*　拡張機能を生成　*/
CREATE EXTENSION "uuid-ossp";
CREATE EXTENSION "pgcrypto";


/**/
ALTER TABLE $(table_name) ENABLE ROW LEVEL SECURITY;

CREATE POLICY $(policy_name) ON $(table_name) [ TO $(role_name) ]
  FOR ALL
  USING ( $(visible_expressions) )
  WITH CHECK ( $() )
;



//
テナント分離の設計にある前提条件は、以下の通り。
１）一つのテナントに一つのデータベースのスキーマ（＝ユーザー）を登録
２）保護するテーブルすべてにテナントIDの列を設けて、パーティションする行にラベルを貼付
３）CREATE POLICY tenant_policy ON $(table_name) USING (tenant_id = current_user());

　テナントを登録する際に専用のデータベースユーザーを登録し、そのテナントの処理を専用のデータベースユーザーで行う。
　この結果、データベースユーザーを誤らない限りは、テナント以外のデータを表示、更新することを防ぐことができる。

　では、鶏卵前後論となるテナントに紐付くデータベースユーザーの名前を何処に保存して参照するのか？
　　→　アカウント管理と同じで、アクセス権限を付与するシーケンスにおいて正確なデータを認証して引き渡す必要がある。
　上記をクリアした上で、実装ミスをどのように防ぐのか？
　→　つまり「ユーザーランドから変更操作を保護した領域から識別情報を入力する問題」を解決する取り組み。
　　→　クライアント／サーバ型の構造の中で、保護するべき情報の責任分解点は、どこになるのか？
　　　→　「利用者を認証して承認した時点」
　　　　→　では、サーバ上の何処に収納して変更から保護するのか？
　　　→　「変更から保護された記憶領域」は？
　　　　→　Webアプリケーションならば、フレームワークが、その任務、役割を負う。
　　★ 認証プロセスのシーケンスにおいて、アプリケーション本体から分離した保護された記憶領域へアクセストークンを記録する。

====================

-- switch role to archons --

CREATE TABLE PAccounts (
  AccountID     UUID DEFAULT gen_random_uuid() NOT NULL,
  Email         VARCHAR(256) NOT NULL,
  Secret        VARCHAR(512) NOT NULL,
  Name          VARCHAR(256) NOT NULL,
  Status        INTEGER NOT NULL,
  ExpireAt      TIMESTAMP,
  UpdateAt      TIMESTAMP,
  CreateAt      TIMESTAMP DEFAULT now(),
  DeleteAt      TIMESTAMP,
  PRIMARY KEY (AccountID),
  FOREIGN KEY (Status) REFERENCES MStatus (Code),
  UNIQUE (Email)
);
GRANT SELECT, REFERENCES ON PAccounts TO aploper;

INSERT INTO PAccounts (Email, Secret, Name, Status) VALUES ('yuna@arteria-s.net', 'yuna', '結城　友菜', 0);


DROP FUNCTION ;
CREATE FUNCTION  (
  pEmail VARCHAR(256) NOT NULL,
  pSecret VARCHAR(512) NOT NULL
) RETURNS UUID AS $$
  SELECT AccountID FROM PAccounts WHERE Email = pEmail AND Secret = pSecret;
$$ LANGUAGE SQL;

CREATE TABLE PTallys (
  AccessToken   UUID DEFAULT gen_random_uuid() NOT NULL,
  AccountID     UUID NOT NULL,
  ExpireAt      TIMESTAMP NOT NULL,
  CreateAt      TIMESTAMP DEFAULT now() NOT NULL,
  PRIMARY KEY (AccessToken),
  FOREIGN KEY (AccountID) REFERENCES PAccounts (AccountID)
    ON DELETE CASCADE ON UPDATE CASCADE,
  UNIQUE (AccountID)
);

DROP FUNCTION CreateTally;
CREATE FUNCTION CreateTally (
  pAccountID    UUID
) RETURNS UUID AS $$
  INSERT INTO PTallys (AccountID, ExpireAt) (SELECT pAccountID, now() + 60);
  SELECT AccessToken FROM PTallys WHERE AccountID = pAccountID;
$$ LANGUAGE SQL;


INSERT INTO PTallys (AccountID, ExpireAt) (SELECT '{a05015c4-fb74-4dec-b39f-399f873a5440}', now() + cast('1 day' as interval));

SELECT AccountID FROM PTallys WHERE AccessToken = 


