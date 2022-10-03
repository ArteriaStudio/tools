/*　現在年度　*/
DROP TABLE MCurrent;
CREATE TABLE MCurrent (
  Year      INTEGER NOT NULL,
  PRIMARY KEY ( Year )
);
GRANT ALL ON MCurrent TO cmnoper;
GRANT SELECT ON MCurrent TO aploper;
INSERT INTO MCurrent ( Year ) VALUES ( 2022 );

/*　市町村テーブル　*/
DROP TABLE MCitys;
CREATE TABLE MCitys (
  CityID    VARCHAR(6) NOT NULL,
  Name      VARCHAR(10) NOT NULL,
  PRIMARY KEY (CityID)
);
GRANT ALL ON MCitys TO cmnoper;
GRANT SELECT ON MCitys TO aploper;

/*　データをインポート　*/
\copy mcitys (cityid, name) from '0001_cities.csv' with csv;

/*　市町村テーブル　*/
DROP TABLE MStates;
CREATE TABLE MStates (
  StateID   VARCHAR(6) NOT NULL,
  Name      VARCHAR(4) NOT NULL,
  PRIMARY KEY (StateID)
);
GRANT ALL ON MStates TO cmnoper;
GRANT SELECT ON MStates TO aploper;

/*　データをインポート　*/
\copy MStates (StateID, Name) from '0002_prefecture.csv' with csv;

/*　アカウント状態　*/
DROP TABLE MStatus;
CREATE TABLE MStatus (
  Ordinal   INTEGER     NOT NULL,
  Code      INTEGER     NOT NULL,
  Text      VARCHAR(16) NOT NULL,
  PRIMARY KEY (Ordinal),
  UNIQUE (Code)
);
GRANT ALL ON MStatus TO cmnoper;
GRANT SELECT ON MStatus TO aploper;

INSERT INTO MStatus (Ordinal, Code, Text) VALUES (0, 0, 'Active');
INSERT INTO MStatus (Ordinal, Code, Text) VALUES (1, 1, 'Suspended');
INSERT INTO MStatus (Ordinal, Code, Text) VALUES (2, 2, 'Archived');

/*　職員種類　*/
DROP TABLE MStaffTypes;
CREATE TABLE MStaffTypes (
  Ordinal   INTEGER     NOT NULL,
  StaffType INTEGER     NOT NULL,
  Text      VARCHAR(16) NOT NULL,
  PRIMARY KEY (Ordinal),
  UNIQUE (StaffType)
);
GRANT ALL ON MStaffTypes TO cmnoper;
GRANT SELECT ON MStaffTypes TO aploper;

INSERT INTO MStaffTypes (Ordinal, StaffType, Text) VALUES (0, 0, 'Unknown');
INSERT INTO MStaffTypes (Ordinal, StaffType, Text) VALUES (1, 1, '教育職員');
INSERT INTO MStaffTypes (Ordinal, StaffType, Text) VALUES (2, 2, '事務職員');
INSERT INTO MStaffTypes (Ordinal, StaffType, Text) VALUES (3, 3, '技術職員');


/*　雇用形態　*/
DROP TABLE MContractTypes;
CREATE TABLE MContractTypes (
  Ordinal       INTEGER     NOT NULL,
  ContractType  INTEGER     NOT NULL,
  Text          VARCHAR(16) NOT NULL,
  PRIMARY KEY (Ordinal),
  UNIQUE (ContractType)
);
GRANT ALL ON MContractTypes TO cmnoper;
GRANT SELECT ON MContractTypes TO aploper;

INSERT INTO MContractTypes (Ordinal, ContractType, Text) VALUES (0, 0, 'Unknown');
INSERT INTO MContractTypes (Ordinal, ContractType, Text) VALUES (1, 1, '専任');
INSERT INTO MContractTypes (Ordinal, ContractType, Text) VALUES (2, 2, '常勤');
INSERT INTO MContractTypes (Ordinal, ContractType, Text) VALUES (3, 3, '非常勤');
INSERT INTO MContractTypes (Ordinal, ContractType, Text) VALUES (4, 4, '嘱託');
INSERT INTO MContractTypes (Ordinal, ContractType, Text) VALUES (5, 5, '派遣');

/*　所属種類　*/
DROP TABLE MMemberTypes;
CREATE TABLE MMemberTypes (
  Ordinal       INTEGER     NOT NULL,
  MemberType    INTEGER     NOT NULL,
  Text          VARCHAR(16) NOT NULL,
  PRIMARY KEY (Ordinal),
  UNIQUE (MemberType)
);
GRANT ALL ON MMemberTypes TO cmnoper;
GRANT SELECT ON MMemberTypes TO aploper;

INSERT INTO MMemberTypes (Ordinal, MemberType, Text) VALUES (0, 0, 'メンバ');
INSERT INTO MMemberTypes (Ordinal, MemberType, Text) VALUES (1, 1, 'マネージャ');
INSERT INTO MMemberTypes (Ordinal, MemberType, Text) VALUES (2, 2, 'オーナ');

/*　役割種類　*/
DROP TABLE MDomains;
CREATE TABLE MDomains (
  Category      INTEGER NOT NULL,
  Name          VARCHAR(256) NOT NULL,
  CreateAt      TIMESTAMP DEFAULT now(),
  UpdateAt      TIMESTAMP,
  DeleteAt      TIMESTAMP,
  PRIMARY KEY (Category),
  UNIQUE (Name)
);
GRANT ALL ON MMemberTypes TO cmnoper;
GRANT SELECT ON MMemberTypes TO aploper;

INSERT INTO MDomains (Category, Name) VALUES (0, '@class.bunri-s.ed.jp');
INSERT INTO MDomains (Category, Name) VALUES (1, '@study.bunri-s.ed.jp');
INSERT INTO MDomains (Category, Name) VALUES (2, '@staff.bunri-s.ed.jp');


/*　組織単位　*/
CREATE TABLE MOrgUnits (
  OrgUnitID     UUID NOT NULL DEFAULT gen_random_uuid(),
  Code          VARCHAR(8) NOT NULL,
  Name          VARCHAR(32) NOT NULL,
  CreateAt      TIMESTAMP DEFAULT now(),
  UpdateAt      TIMESTAMP,
  DeleteAt      TIMESTAMP,
  PRIMARY KEY ( OrgUnitID ),
  UNIQUE ( Code ),
  UNIQUE ( Name ) 
);
GRANT ALL ON MOrgUnits TO cmnoper;
GRANT SELECT ON MOrgUnits TO aploper;

/*　　組織構造　*/
CREATE TABLE MOrgRels (
  Year          INTEGER NOT NULL,
  OrgUnitID     UUID NOT NULL,
  ContainerID   UUID NOT NULL,
  CreateAt      TIMESTAMP DEFAULT now(),
  UpdateAt      TIMESTAMP,
  DeleteAt      TIMESTAMP,
  PRIMARY KEY ( Year, OrgUnitID ),
  FOREIGN KEY ( OrgUnitID ) REFERENCES MOrgUnits ( OrgUnitID )
    ON DELETE CASCADE ON UPDATE CASCADE
);
GRANT ALL ON MOrgRels TO cmnoper;
GRANT SELECT ON MOrgRels TO aploper;

/*　組織単位を追加　*/
CREATE PROCEDURE AppendOrgUnit (pYear INTEGER, pCode VARCHAR(8), pName VARCHAR(32), pContainerID UUID)
LANGUAGE SQL AS $$
  INSERT INTO MOrgUnits (Code, Name) VALUES (pCode, pName);
  INSERT INTO MOrgRels (Year, OrgUnitID, ContainerID) (SELECT pYear, OrgUnitID, pContainerID FROM MOrgUnits WHERE Code = pCode);
$$;

CREATE PROCEDURE MoveOrgUnit (pYear INTEGER, pCode VARCHAR(8), pContainerCode VARCHAR(8))
LANGUAGE SQL AS $$
  UPDATE MOrgRels SET ContainerID = (SELECT OrgUnitID FROM MOrgUnits WHERE Code = pContainerCode) WHERE OrgUnitID = (SELECT OrgUnitID FROM MOrgUnits WHERE Code = pCode);
$$;

CREATE FUNCTION GetRootOrgUnitID() RETURNS UUID
LANGUAGE SQL AS $$
  SELECT OrgUnitID FROM MOrgRels WHERE Year = (SELECT Year FROM MCurrent) AND ContainerID = '{00000000-0000-0000-0000-000000000000}';
$$;

/*　アカウント基本情報　*/
DROP TABLE MAccounts;
CREATE TABLE MAccounts (
  AccountID     UUID         NOT NULL DEFAULT gen_random_uuid(),
  Email         VARCHAR(256) NOT NULL,
  Name          VARCHAR(256) NOT NULL,
  Read          VARCHAR(256) NOT NULL,
  OrgUnitID     UUID         NOT NULL,
  Status        INTEGER      NOT NULL,
  ExpireAt      TIMESTAMP    DEFAULT NULL,
  UpdateAt      TIMESTAMP    DEFAULT NULL,
  DeleteAt      TIMESTAMP    DEFAULT NULL,
  PRIMARY KEY (AccountID),
  FOREIGN KEY (Status) REFERENCES MStatus (Code)
    ON DELETE CASCADE ON UPDATE CASCADE,
  UNIQUE (Email)
);
GRANT ALL ON MAccounts TO cmnoper;
GRANT SELECT ON MAccounts TO aploper;

DROP PROCEDURE UpsertAccount;
CREATE PROCEDURE UpsertAccount (
  pEmail      VARCHAR(256),
  pName       VARCHAR(256),
  pRead       VARCHAR(256),
  pOrgUnit    UUID,
  iStatus     INTEGER)
LANGUAGE SQL AS $$
  INSERT INTO MAccounts (Email, Name, Read, OrgUnitID, Status)
    (SELECT pEmail, pName, pRead, pOrgUnit, iStatus)
  ON CONFLICT ON CONSTRAINT maccounts_email_key
  DO UPDATE SET Email = pEMail, Name = pName, Read = pRead, OrgUnitID = pOrgUnit, Status = iStatus;
$$;

DROP FUNCTION GetAccountIDByEmail;
CREATE FUNCTION GetAccountIDByEmail (
  VARCHAR(256)
) RETURNS UUID AS $$
  SELECT AccountID FROM MAccounts WHERE Email = $1
$$ LANGUAGE SQL;

/*　生徒用メールアドレス生成　*/
DROP FUNCTION GenerateEmailForStudent;
CREATE FUNCTION GenerateEmailForStudent (
  pStudentNumber VARCHAR(5)
) RETURNS VARCHAR(256) AS $$
  SELECT pStudentNumber || Name FROM MDomains WHERE Category = 0;
$$ LANGUAGE SQL;





/*　旧姓　*/
DROP TABLE MOriginalNames;
CREATE TABLE MOriginalNames (
  AccountID     UUID NOT NULL,
  Family        VARCHAR(60) NOT NULL,
  ReadFamily    VARCHAR(60) NOT NULL,
  PRIMARY KEY ( AccountID ),
  FOREIGN KEY ( AccountID ) REFERENCES MAccounts ( AccountID )
    ON DELETE CASCADE ON UPDATE CASCADE
);
GRANT ALL ON MOriginalNames TO cmnoper;
GRANT SELECT ON MOriginalNames TO aploper;

CREATE PROCEDURE AddOriginalName (
  pEmail      VARCHAR(256),
  pName       VARCHAR(256),
  pRead       VARCHAR(256),
  pOrgUnit    UUID,
  iStatus     INTEGER,
  pFamily     VARCHAR(60),
  pReadFamily VARCHAR(60)
)
LANGUAGE SQL AS $$
  CALL UpsertAccount(pEmail, pName, pRead, pOrgUnit, iStatus);
  INSERT INTO MOriginalNames (AccountID, Family, ReadFamily)
    (SELECT AccountID, pFamily, pReadFamily FROM MAccounts WHERE Email = pEmail);
$$;

CALL AddOriginalName('yuna@arteria-s.net', '結城　友菜', 'ゆうき　ゆうな', GetRootOrgUnitID(), 0, '結城', 'ゆうき');


/*　戸籍名　*/
CREATE TABLE MRegisterNames (
  AccountID     UUID NOT NULL,
  First         VARCHAR(60) NOT NULL,
  Family        VARCHAR(60) NOT NULL,
  ReadFirst     VARCHAR(60) NOT NULL,
  ReadFamily    VARCHAR(60) NOT NULL,
  PRIMARY KEY ( AccountID ),
  FOREIGN KEY ( AccountID ) REFERENCES MAccounts ( AccountID )
    ON DELETE CASCADE ON UPDATE CASCADE
);
GRANT ALL ON MRegisterNames TO cmnoper;
GRANT SELECT ON MRegisterNames TO aploper;

CREATE PROCEDURE AddRegisterName (
  pEmail      VARCHAR(256),
  pName       VARCHAR(256),
  pRead       VARCHAR(256),
  pOrgUnit    UUID,
  iStatus     INTEGER,
  pFirst      VARCHAR(60),
  pFamily     VARCHAR(60),
  pReadFirst  VARCHAR(60),
  pReadFamily VARCHAR(60)
)
LANGUAGE SQL AS $$
  CALL UpsertAccount(pEmail, pName, pRead, pOrgUnit, iStatus);
  INSERT INTO MRegisterNames (AccountID, First, Family, ReadFirst, ReadFamily)
    (SELECT AccountID, pFirst, pFamily, pReadFirst, pReadFamily FROM MAccounts WHERE Email = pEmail);
$$;

CALL AddRegisterName('yuna@arteria-s.net', '結城　友菜', 'ゆうき　ゆうな', GetRootOrgUnitID(), 0, '友菜', '結城', 'ゆうな', 'ゆうき');


/*　通名　*/
CREATE TABLE MPopularNames (
  AccountID     UUID NOT NULL,
  First         VARCHAR(60) NOT NULL,
  Family        VARCHAR(60) NOT NULL,
  ReadFirst     VARCHAR(60) NOT NULL,
  ReadFamily    VARCHAR(60) NOT NULL,
  PRIMARY KEY ( AccountID ),
  FOREIGN KEY ( AccountID ) REFERENCES MAccounts ( AccountID )
    ON DELETE CASCADE ON UPDATE CASCADE
);
GRANT ALL ON MPopularNames TO cmnoper;
GRANT SELECT ON MPopularNames TO aploper;

CREATE PROCEDURE AddPopularName (
  pEmail      VARCHAR(256),
  pName       VARCHAR(256),
  pRead       VARCHAR(256),
  pOrgUnit    UUID,
  iStatus     INTEGER,
  pFirst      VARCHAR(60),
  pFamily     VARCHAR(60),
  pReadFirst  VARCHAR(60),
  pReadFamily VARCHAR(60)
)
LANGUAGE SQL AS $$
  CALL UpsertAccount(pEmail, pName, pRead, pOrgUnit, iStatus);
  INSERT INTO MPopularNames (AccountID, First, Family, ReadFirst, ReadFamily)
    (SELECT AccountID, pFirst, pFamily, pReadFirst, pReadFamily FROM MAccounts WHERE Email = pEmail);
$$;

CALL AddPopularName('yuna@arteria-s.net', '結城　友菜', 'ゆうき　ゆうな', GetRootOrgUnitID(), 0, '友菜', '結城', 'ゆうな', 'ゆうき');


/*　常用名　*/
CREATE TABLE MRegularNames (
  AccountID     UUID NOT NULL,
  First         VARCHAR(60) NOT NULL,
  Family        VARCHAR(60) NOT NULL,
  PRIMARY KEY ( AccountID ),
  FOREIGN KEY ( AccountID ) REFERENCES MAccounts ( AccountID )
    ON DELETE CASCADE ON UPDATE CASCADE
);
GRANT ALL ON MRegularNames TO cmnoper;
GRANT SELECT ON MRegularNames TO aploper;

CREATE PROCEDURE AddRegularName (
  pEmail      VARCHAR(256),
  pName       VARCHAR(256),
  pRead       VARCHAR(256),
  pOrgUnit    UUID,
  iStatus     INTEGER,
  pFirst      VARCHAR(60),
  pFamily     VARCHAR(60)
)
LANGUAGE SQL AS $$
  CALL UpsertAccount(pEmail, pName, pRead, pOrgUnit, iStatus);
  INSERT INTO MRegularNames (AccountID, First, Family)
    (SELECT AccountID, pFirst, pFamily FROM MAccounts WHERE Email = pEmail);
$$;

CALL AddRegularName('yuna@arteria-s.net', '結城　友菜', 'ゆうき　ゆうな', GetRootOrgUnitID(), 0, '友菜', '結城');


/*　住所　*/
CREATE TABLE MAddress (
  AccountID     UUID NOT NULL,
  Address       VARCHAR(64) NOT NULL,
  CityID        VARCHAR(6) NOT NULL,
  PRIMARY KEY ( AccountID ),
  FOREIGN KEY ( AccountID ) REFERENCES MAccounts ( AccountID )
    ON DELETE CASCADE ON UPDATE CASCADE,
  FOREIGN KEY ( CityID ) REFERENCES MCitys ( CityID )
    ON DELETE CASCADE ON UPDATE CASCADE
);
GRANT ALL ON MAddress TO cmnoper;
GRANT SELECT ON MAddress TO aploper;

CREATE PROCEDURE AddAddress (
  pEmail        VARCHAR(256),
  pName         VARCHAR(256),
  pRead         VARCHAR(256),
  pOrgUnit      UUID,
  iStatus       INTEGER,
  pAddress      VARCHAR(64),
  pCityID       VARCHAR(6)
)
LANGUAGE SQL AS $$
  CALL UpsertAccount(pEmail, pName, pRead, pOrgUnit, iStatus);
  INSERT INTO MAddress (AccountID, Address, CityID)
    (SELECT AccountID, pAddress, pCityID FROM MAccounts WHERE Email = pEmail);
$$;

/*
CALL AddPopularName('yuna@arteria-s.net', '結城　友菜', 'ゆうき　ゆうな', GetRootOrgUnitID(), 0, 'm', 372048);
*/





/*　職員　*/
DROP TABLE MEmploys;
CREATE TABLE MEmploys (
  StaffNumber   VARCHAR(5) NOT NULL,
  AccountID     UUID NOT NULL,
  StaffType     INTEGER NOT NULL,
  ContractType  INTEGER NOT NULL,
  PRIMARY KEY ( StaffNumber ),
  FOREIGN KEY ( AccountID ) REFERENCES MAccounts ( AccountID )
    ON DELETE CASCADE ON UPDATE CASCADE,
  FOREIGN KEY ( StaffType ) REFERENCES MStaffTypes ( StaffType )
    ON DELETE CASCADE ON UPDATE CASCADE,
  FOREIGN KEY ( ContractType ) REFERENCES MContractTypes ( ContractType )
    ON DELETE CASCADE ON UPDATE CASCADE
);
GRANT ALL ON MEmploys TO cmnoper;
GRANT SELECT ON MEmploys TO aploper;

CREATE PROCEDURE UpsertEmploy (
  pStaffNumber  VARCHAR(5),
  pEmail        VARCHAR(256),
  pStaffType    INTEGER,
  pContractType INTEGER
) LANGUAGE SQL AS $$
  INSERT INTO MEmploys (AccountID, StaffNumber, StaffType, ContractType)
    (SELECT AccountID, pStaffNumber, pStaffType, pContractType FROM MAccounts WHERE Email = pEmail)
  ON CONFLICT ON CONSTRAINT memploys_pkey
  DO UPDATE SET StaffNumber = pStaffNumber, StaffType = pStaffType, ContractType = pContractType;
$$;

CALL UpsertEmploy('1011', 'rink@arteria-s.net', 1, 2);


/*　学籍　*/
DROP TABLE MStudents;
CREATE TABLE MStudents (
  StudentNumber VARCHAR(5) NOT NULL,
  AccountID     UUID NOT NULL,
  Gender        VARCHAR(1) NOT NULL,
  BirthAt       TIMESTAMP NOT NULL,
  EnterAt       TIMESTAMP NOT NULL DEFAULT now(),
  LeaveAt       TIMESTAMP,
  PRIMARY KEY ( StudentNumber ),
  FOREIGN KEY ( AccountID ) REFERENCES MAccounts ( AccountID )
    ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT CK_GENDER CHECK ( Gender = '男' OR Gender = '女' )
);
GRANT ALL ON MStudents TO cmnoper;
GRANT SELECT ON MStudents TO aploper;

/*　学籍マスタは現状だと更新する契機が見当たらないのでストアドプロシージャはなし（2022/09/20）　*/

INSERT INTO MStudents (StudentNumber, AccountID, Gender, BirthAt) (SELECT '00000', AccountID, '女', now() FROM MAccounts WHERE Email = 'wakaba@arteria-s.net');
INSERT INTO MStudents (StudentNumber, AccountID, Gender, BirthAt) (SELECT '00099', AccountID, '女', now() FROM MAccounts WHERE Email = 'yuna@arteria-s.net');



/* 学年 */
DROP TABLE MGrade;
CREATE TABLE MGrade (
  Year          INTEGER NOT NULL,
  School        VARCHAR(2) NOT NULL,
  Grade         VARCHAR(1) NOT NULL,
  Sets          VARCHAR(2) NOT NULL,
  Numbers       VARCHAR(2) NOT NULL,
  AccountID     UUID NOT NULL,
  PRIMARY KEY ( Year, School, Grade, Sets, Numbers ),
  FOREIGN KEY ( AccountID ) REFERENCES MAccounts ( AccountID )
    ON DELETE CASCADE ON UPDATE CASCADE,
  FOREIGN KEY ( Year ) REFERENCES MCurrent ( Year )
    ON DELETE CASCADE ON UPDATE CASCADE
);
GRANT ALL ON MGrade TO cmnoper;
GRANT SELECT ON MGrade TO aploper;

/*　注意：未登録を検出する方法を実装していないので、実質このストアドプロシージャは使ってはならない（2022/09/20）　*/
CREATE PROCEDURE UpsertGrade (
  pYear          INTEGER,
  pStudentNumber VARCHAR(5),
  pSchool        VARCHAR(2),
  pGrade         VARCHAR(1),
  pSets          VARCHAR(2),
  pNumbers       VARCHAR(2)
) LANGUAGE SQL AS $$
  INSERT INTO MGrade (Year, School, Grade, Sets, Numbers, AccountID)
    (SELECT pYear, pSchool, pGrade, pSets, pNumbers, AccountID FROM MStudents WHERE StudentNumber = pStudentNumber)
  ON CONFLICT ON CONSTRAINT mgrade_pkey
  DO UPDATE SET Year = pYear, School = pSchool, Grade = pGrade, Sets = pSets, Numbers = pNumbers;
$$;

CALL UpsertGrade('2022', '00000', '中学', '1', '1', '1');
CALL UpsertGrade('2022', '00099', '中学', '3', '6', '99');



/*　付帯　*/
DROP TABLE MAlignments;
CREATE TABLE MAlignments (
  AccountID     UUID NOT NULL,
  License       INTEGER NOT NULL,
  GmailID       VARCHAR(256) NOT NULL,
  MicrosoftID   VARCHAR(256),
  Sequence      VARCHAR(6),
  SSNCode       VARCHAR(24),
  SSECode       VARCHAR(24),
  PRIMARY KEY ( AccountID ),
  FOREIGN KEY ( AccountID ) REFERENCES MAccounts ( AccountID )
    ON DELETE CASCADE ON UPDATE CASCADE,
  UNIQUE ( GmailID ),
  UNIQUE ( MicrosoftID ),
  UNIQUE ( SSNCode ),
  UNIQUE ( SSECode )
);
GRANT ALL ON MAlignments TO cmnoper;
GRANT SELECT ON MAlignments TO aploper;


/*　集団　*/
DROP TABLE MGroups;
CREATE TABLE MGroups (
  GroupID       UUID NOT NULL DEFAULT gen_random_uuid(),
  Email         VARCHAR(256) NOT NULL,
  GroupName     VARCHAR(60) NOT NULL,
  CreateAt      TIMESTAMP DEFAULT now(),
  UpdateAt      TIMESTAMP,
  DeleteAt      TIMESTAMP,
  PRIMARY KEY ( GroupID ),
  UNIQUE ( Email ),
  UNIQUE ( GroupName )
);
GRANT ALL ON MGroups TO cmnoper;
GRANT SELECT ON MGroups TO aploper;

DROP PROCEDURE UpsertGroup;
CREATE PROCEDURE UpsertGroup (
  pEmail        VARCHAR(256),
  pGroupName    VARCHAR(60)
) LANGUAGE SQL AS $$
  INSERT INTO MGroups (Email, GroupName) (SELECT pEmail, pGroupName)
  ON CONFLICT ON CONSTRAINT mgroups_email_key
  DO UPDATE SET Email = pEmail, GroupName = pGroupName;
$$;

CALL UpsertGroup('hermelin@arteria-s.net', '機能検証チーム');



/*　所属　*/
CREATE TABLE MMembers (
  GroupID       UUID NOT NULL,
  AccountID     UUID NOT NULL,
  MemberType    INTEGER NOT NULL,
  PRIMARY KEY ( GroupID ),
  FOREIGN KEY ( GroupID ) REFERENCES MGroups ( GroupID )
    ON DELETE CASCADE ON UPDATE CASCADE,
  FOREIGN KEY ( MemberType ) REFERENCES MMemberTypes ( MemberType )
    ON DELETE CASCADE ON UPDATE CASCADE,
  FOREIGN KEY ( AccountID ) REFERENCES MAccounts ( AccountID )
    ON DELETE CASCADE ON UPDATE CASCADE
);
GRANT ALL ON MMembers TO cmnoper;
GRANT SELECT ON MMembers TO aploper;

DROP PROCEDURE UpsertMember;
CREATE PROCEDURE UpsertMember (
  pEmail        VARCHAR(256),
  pAccountID    UUID,
  iMemberType   INTEGER
) LANGUAGE SQL AS $$
  INSERT INTO MMembers (GroupID, AccountID, MemberType) (SELECT GroupID, pAccountID, iMemberType FROM MGroups WHERE Email = pEmail)
  ON CONFLICT ON CONSTRAINT mmembers_pkey
  DO UPDATE SET AccountID = pAccountID, MemberType = iMemberType;
$$;

CALL UpsertMember('hermelin@arteria-s.net', '{00470259-3371-47c7-b760-63e122fc293d}', 1);


DROP PROCEDURE UpsertMemberByEmail;
CREATE PROCEDURE UpsertMemberByEmail (
  pEmail        VARCHAR(256),
  pMemberEmail  VARCHAR(256),
  iMemberType   INTEGER
) LANGUAGE SQL AS $$
  CALL UpsertMember(pEmail, GetAccountIDByEmail(pMemberEmail), iMemberType);
$$;

CALL UpsertMemberByEmail('hermelin@arteria-s.net', 'wakaba@arteria-s.net', 2);
