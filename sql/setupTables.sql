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
GRANT SELECT, REFERENCES ON MStatus TO aploper, archons;

INSERT INTO MStatus (Ordinal, Code, Text) VALUES (0, 0, 'Active');
INSERT INTO MStatus (Ordinal, Code, Text) VALUES (1, 1, 'Suspended');
INSERT INTO MStatus (Ordinal, Code, Text) VALUES (2, 2, 'Archived');

CREATE FUNCTION GetStatusCode (
  pStatusName   VARCHAR(16)
) RETURNS INTEGER
LANGUAGE SQL AS $$
  SELECT Code FROM MStatus WHERE Text = pStatusName;
$$;


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

CREATE FUNCTION GetStaffTypeCode (
  pStaffTypeName    VARCHAR(16)
) RETURNS INTEGER
LANGUAGE SQL AS $$
  SELECT StaffType FROM MStaffTypes WHERE Text = pStaffTypeName;
$$;


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

CREATE FUNCTION GetContractTypeCode (
  pContractTypeName     VARCHAR(16)
) RETURNS INTEGER
LANGUAGE SQL AS $$
  SELECT ContractType FROM MContractTypes WHERE Text = pContractTypeName;
$$;


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

CREATE FUNCTION GetMemberTypeCode (
  pMemberTypeName     VARCHAR(16)
) RETURNS INTEGER
LANGUAGE SQL AS $$
  SELECT MemberType FROM MMemberTypes WHERE Text = pMemberTypeName;
$$;


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
DROP TABLE MOrgUnits;
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
GRANT SELECT, INSERT, UPDATE, DELETE ON MOrgUnits TO aploper;

INSERT INTO MOrgUnits (Code, Name) VALUES ('00000001', '生徒');
INSERT INTO MOrgUnits (Code, Name) VALUES ('00000002', '職員');


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
GRANT SELECT, INSERT, UPDATE, DELETE ON MOrgRels TO aploper;

/*　組織単位を追加　*/
CREATE PROCEDURE AppendOrgUnit (pYear INTEGER, pCode VARCHAR(8), pName VARCHAR(32), pContainerID UUID)
LANGUAGE SQL AS $$
  INSERT INTO MOrgUnits (Code, Name) VALUES (pCode, pName);
  INSERT INTO MOrgRels (Year, OrgUnitID, ContainerID) (SELECT pYear, OrgUnitID, pContainerID FROM MOrgUnits WHERE Code = pCode);
$$;

CREATE FUNCTION GetOrgUnitID (
  pContainerCode VARCHAR(8)
) RETURNS UUID
LANGUAGE SQL AS $$
  SELECT OrgUnitID FROM MOrgUnits WHERE Code = pContainerCode;
$$;

DROP PROCEDURE UpsertOrgUnit;
CREATE PROCEDURE UpsertOrgUnit (pYear INTEGER, pCode VARCHAR(8), pName VARCHAR(32), pContainerID UUID)
LANGUAGE SQL AS $$
  INSERT INTO MOrgUnits (Code, Name) VALUES (pCode, pName)
  ON CONFLICT ON CONSTRAINT morgunits_code_key
  DO UPDATE SET Name = pName;
  INSERT INTO MOrgRels (Year, OrgUnitID, ContainerID) (SELECT pYear, OrgUnitID, pContainerID FROM MOrgUnits WHERE Code = pCode)
  ON CONFLICT ON CONSTRAINT morgrels_pkey
  DO UPDATE SET ContainerID = pContainerID;
$$;

DROP PROCEDURE UpdateOrgUnit;
CREATE PROCEDURE UpdateOrgUnit (pOrgUnitID UUID, pCode VARCHAR(8), pName VARCHAR(32))
LANGUAGE SQL AS $$
  UPDATE MOrgUnits SET Code = pCode, Name = pName WHERE OrgUnitID = pOrgUnitID;
$$;

DROP PROCEDURE UpdateOrgUnitContainer;
CREATE PROCEDURE UpdateOrgUnitContainer (iYear INTEGER, pOrgUnitID UUID, pContainerID UUID)
LANGUAGE SQL AS $$
  UPDATE MOrgRels SET ContainerID = pContainerID WHERE Year = iYear AND OrgUnitID = pOrgUnitID;
$$;



CREATE PROCEDURE AppendOrgUnitByCode (pYear INTEGER, pCode VARCHAR(8), pName VARCHAR(32), pContainerCode VARCHAR(8))
LANGUAGE SQL AS $$
  INSERT INTO MOrgUnits (Code, Name) VALUES (pCode, pName);
  INSERT INTO MOrgRels (Year, OrgUnitID, ContainerID) (SELECT pYear, OrgUnitID, GetOrgUnitID(pContainerCode) FROM MOrgUnits WHERE Code = pCode);
$$;


INSERT INTO MOrgUnits (OrgUnitID, Code, Name) VALUES ('{00000000-0000-0000-0000-000000000001}', '00000000', 'アルテリア工房');
INSERT INTO MOrgRels (Year, OrgUnitID, ContainerID) (SELECT 2022, OrgUnitID, '{00000000-0000-0000-0000-000000000000}' FROM MOrgUnits WHERE Code = '00000000');

CALL AppendOrgUnit(2022, '00000001', '生徒', '{00000000-0000-0000-0000-000000000001}');
CALL AppendOrgUnit(2022, '00000002', '職員', '{00000000-0000-0000-0000-000000000001}');

CALL AppendOrgUnitByCode(2022, '00000003', '教育職員', '00000002');
CALL AppendOrgUnitByCode(2022, '00000004', '事務職員', '00000002');
CALL AppendOrgUnitByCode(2022, '00000005', '1期生', '00000001');
CALL AppendOrgUnitByCode(2022, '00000006', '2期生', '00000001');
CALL AppendOrgUnitByCode(2022, '00000009', '21期生', '00000006');



/*
CALL AppendOrgUnit(2022, '________', '1', '{00000000-0000-0000-0000-000000000001}');
CALL AppendOrgUnit(2022, '_', '2', '{00000000-0000-0000-0000-000000000001}');
CALL AppendOrgUnit(2022, ' ', '3', '{00000000-0000-0000-0000-000000000001}');
CALL AppendOrgUnit(2022, '  ', '4', '{00000000-0000-0000-0000-000000000001}');
*/
/*
SELECT MOrgUnits.OrgUnitID, MOrgUnits.Code, MOrgUnits.Name, MOrgRels.ContainerID FROM MOrgUnits INNER JOIN MOrgRels ON MOrgRels.OrgUnitID = MOrgUnits.OrgUnitID;
SELECT MOrgUnits.OrgUnitID, MOrgUnits.Code, MOrgUnits.Name, MOrgRels.ContainerID FROM MOrgUnits LEFT OUTER JOIN MOrgRels ON MOrgRels.OrgUnitID = MOrgUnits.OrgUnitID;
*/

CREATE PROCEDURE MoveOrgUnit (pYear INTEGER, pCode VARCHAR(8), pContainerCode VARCHAR(8))
LANGUAGE SQL AS $$
  UPDATE MOrgRels SET ContainerID = (SELECT OrgUnitID FROM MOrgUnits WHERE Code = pContainerCode) WHERE OrgUnitID = (SELECT OrgUnitID FROM MOrgUnits WHERE Code = pCode);
$$;

CREATE FUNCTION GetRootOrgUnitID() RETURNS UUID
LANGUAGE SQL AS $$
  SELECT OrgUnitID FROM MOrgRels WHERE Year = (SELECT Year FROM MCurrent) AND ContainerID = '{00000000-0000-0000-0000-000000000001}';
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
GRANT SELECT, INSERT, UPDATE ON MAccounts TO aploper;

DROP PROCEDURE InsertAccount;
CREATE PROCEDURE InsertAccount (
  pEmail      VARCHAR(256),
  pName       VARCHAR(256),
  pRead       VARCHAR(256),
  pOrgUnit    UUID,
  iStatus     INTEGER)
LANGUAGE SQL AS $$
  INSERT INTO MAccounts (Email, Name, Read, OrgUnitID, Status)
    (SELECT pEmail, pName, pRead, pOrgUnit, iStatus)
$$;


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
GRANT SELECT, INSERT, UPDATE, DELETE ON MOriginalNames TO aploper;

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
GRANT SELECT, INSERT, UPDATE, DELETE ON MRegisterNames TO aploper;

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
GRANT SELECT, INSERT, UPDATE, DELETE ON MPopularNames TO aploper;

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
GRANT SELECT, INSERT, UPDATE, DELETE ON MRegularNames TO aploper;

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
GRANT SELECT, INSERT, UPDATE, DELETE ON MAddress TO aploper;

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
GRANT SELECT, INSERT, UPDATE, DELETE ON MEmploys TO aploper;

DROP PROCEDURE UpsertEmploy;
CREATE PROCEDURE UpsertEmploy (
  pEmail        VARCHAR(256),
  pStaffNumber  VARCHAR(5),
  pStaffType    INTEGER,
  pContractType INTEGER
) LANGUAGE SQL AS $$
  INSERT INTO MEmploys (AccountID, StaffNumber, StaffType, ContractType)
    (SELECT AccountID, pStaffNumber, pStaffType, pContractType FROM MAccounts WHERE Email = pEmail)
  ON CONFLICT ON CONSTRAINT memploys_pkey
  DO UPDATE SET StaffNumber = pStaffNumber, StaffType = pStaffType, ContractType = pContractType;
$$;

CALL UpsertEmploy('rink@arteria-s.net', '1011', 1, 2);


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
GRANT SELECT, INSERT, UPDATE, DELETE ON MStudents TO aploper;

DROP PROCEDURE InsertStudent;
CREATE PROCEDURE InsertStudent (
  pStudentNumber    VARCHAR(5),
  pAccountID        UUID,
  pGender           VARCHAR(1),
  pBirthAt          TIMESTAMP,
  pEnterAt          TIMESTAMP
) LANGUAGE SQL AS $$
  INSERT INTO MStudents (StudentNumber, AccountID, Gender, BirthAt, EnterAt)
    (SELECT pStudentNumber, pAccountID, pGender, pBirthAt, pEnterAt)
$$;

DROP PROCEDURE UpsertStudent;
CREATE PROCEDURE UpsertStudent (
  pStudentNumber    VARCHAR(5),
  pAccountID        UUID,
  pGender           VARCHAR(1),
  pBirthAt          TIMESTAMP,
  pEnterAt          TIMESTAMP
) LANGUAGE SQL AS $$
  INSERT INTO MStudents (StudentNumber, AccountID, Gender, BirthAt, EnterAt)
    (SELECT pStudentNumber, pAccountID, pGender, pBirthAt, pEnterAt)
  ON CONFLICT ON CONSTRAINT mstudents_pkey
  DO UPDATE SET Gender = pGender, BirthAt = pBirthAt, EnterAt = pEnterAt;
$$;

/*　学籍マスタは現状だと更新する契機が見当たらないのでストアドプロシージャはなし（2022/09/20）　*/

INSERT INTO MStudents (StudentNumber, AccountID, Gender, BirthAt) (SELECT '00000', AccountID, '女', now() FROM MAccounts WHERE Email = 'wakaba@arteria-s.net');
INSERT INTO MStudents (StudentNumber, AccountID, Gender, BirthAt) (SELECT '00099', AccountID, '女', now() FROM MAccounts WHERE Email = 'yuna@arteria-s.net');



/* 学年 */
DROP TABLE MGrade;
CREATE TABLE MGrade (
  StudentNumber VARCHAR(5) NOT NULL,
  Year          INTEGER NOT NULL,
  School        VARCHAR(2) NOT NULL,
  Grade         VARCHAR(1) NOT NULL,
  Sets          VARCHAR(2) NOT NULL,
  Numbers       VARCHAR(2) NOT NULL,
  AccountID     UUID NOT NULL,
  PRIMARY KEY ( StudentNumber, Year ),
  FOREIGN KEY ( AccountID ) REFERENCES MAccounts ( AccountID )
    ON DELETE CASCADE ON UPDATE CASCADE,
  UNIQUE ( Year, School, Grade, Sets, Numbers )
);
GRANT ALL ON MGrade TO cmnoper;
GRANT SELECT, INSERT, UPDATE ON MGrade TO aploper;

DROP PROCEDURE InsertGrade;
CREATE PROCEDURE InsertGrade (
  iYear          INTEGER,
  pStudentNumber VARCHAR(5),
  pSchool        VARCHAR(2),
  pGrade         VARCHAR(1),
  pSets          VARCHAR(2),
  pNumbers       VARCHAR(2)
) LANGUAGE SQL AS $$
  INSERT INTO MGrade (StudentNumber, Year, School, Grade, Sets, Numbers, AccountID)
    (SELECT pStudentNumber, iYear, pSchool, pGrade, pSets, pNumbers, AccountID FROM MStudents WHERE StudentNumber = pStudentNumber)
$$;





/*　注意：未登録を検出する方法を実装していないので、実質このストアドプロシージャは使ってはならない（2022/09/20）　*/
DROP PROCEDURE UpsertGrade;
CREATE PROCEDURE UpsertGrade (
  iYear          INTEGER,
  pStudentNumber VARCHAR(5),
  pSchool        VARCHAR(3),
  pGrade         VARCHAR(1),
  pSets          VARCHAR(2),
  pNumbers       VARCHAR(2)
) LANGUAGE SQL AS $$
  INSERT INTO MGrade (StudentNumber, Year, School, Grade, Sets, Numbers, AccountID)
    (SELECT pStudentNumber, iYear, pSchool, pGrade, pSets, pNumbers, AccountID FROM MStudents WHERE StudentNumber = pStudentNumber)
  ON CONFLICT ON CONSTRAINT mgrade_pkey
  DO UPDATE SET Year = iYear, School = pSchool, Grade = pGrade, Sets = pSets, Numbers = pNumbers;
$$;

CALL UpsertGrade('2022', '00000', '中学', '1', '1', '1');
CALL UpsertGrade('2022', '00099', '中学', '3', '6', '99');


DROP PROCEDURE InsertStudentAccount;
CREATE PROCEDURE InsertStudentAccount (
  pOrgUnitCode   VARCHAR(8),
  pEmail         VARCHAR(256),
  pName          VARCHAR(256),
  pRead          VARCHAR(256),
  pGender        VARCHAR(1),
  pBirthAt       TIMESTAMP,
  pEnterAt       TIMESTAMP,
  iYear          INTEGER,
  pStudentNumber VARCHAR(5),
  pSchool        VARCHAR(3),
  pGrade         VARCHAR(1),
  pSets          VARCHAR(2),
  pNumbers       VARCHAR(2)
) LANGUAGE 'plpgsql' AS $$
DECLARE
  pOrgUnitID     UUID;
  pAccountID     UUID;
BEGIN
  pOrgUnitID = GetOrgUnitID(pOrgUnitCode);
  CALL InsertAccount(pEmail, pName, pRead, pOrgUnitID, 0);
  pAccountID = GetAccountIDByEmail(pEmail);
  CALL InsertStudent(pStudentNumber, pAccountID, pGender, pBirthAt, pEnterAt);
  CALL InsertGrade(iYear, pStudentNumber, pSchool, pGrade, pSets, pNumbers);
END
$$;

DROP PROCEDURE UpsertStudentAccount;
CREATE PROCEDURE UpsertStudentAccount (
  pOrgUnitCode   VARCHAR(8),
  pEmail         VARCHAR(256),
  pName          VARCHAR(256),
  pRead          VARCHAR(256),
  pGender        VARCHAR(1),
  pBirthAt       TIMESTAMP,
  pEnterAt       TIMESTAMP,
  iYear          INTEGER,
  pStudentNumber VARCHAR(5),
  pSchool        VARCHAR(3),
  pGrade         VARCHAR(1),
  pSets          VARCHAR(2),
  pNumbers       VARCHAR(2)
) LANGUAGE 'plpgsql' AS $$
DECLARE
  pOrgUnitID     UUID;
  pAccountID     UUID;
BEGIN
  pOrgUnitID = GetOrgUnitID(pOrgUnitCode);
  CALL UpsertAccount(pEmail, pName, pRead, pOrgUnitID, 0);
  pAccountID = GetAccountIDByEmail(pEmail);
  CALL UpsertStudent(pStudentNumber, pAccountID, pGender, pBirthAt, pEnterAt);
  CALL UpsertGrade(iYear, pStudentNumber, pSchool, pGrade, pSets, pNumbers);
END
$$;

CALL InsertStudentAccount('000000', 'aes@arteria-s.net', '宮部　みゆき', 'みやべ　みゆき', '女', '2000/01/2', '2022/12/22', 2022, '01001', '高校', '2', 'G2', '2');
CALL UpsertStudentAccount('000000', 'aes@arteria-s.net', '宮部　みゆき', 'みやべ　みゆき', '女', '2000/01/2', '2022/12/22', 2022, '01001', '高校', '2', 'G2', '2');



/*　付帯　*/
DROP TABLE MAlignments;
CREATE TABLE MAlignments (
  AccountID     UUID NOT NULL,
  License       INTEGER NOT NULL,
  WindowsID     VARCHAR(256) NOT NULL,
  GmailID       VARCHAR(256) NOT NULL,
  MicrosoftID   VARCHAR(256) NOT NULL,
  Sequence      VARCHAR(6),
  SSNCode       VARCHAR(24),
  SSECode       VARCHAR(24),
  PRIMARY KEY ( AccountID ),
  FOREIGN KEY ( AccountID ) REFERENCES MAccounts ( AccountID )
    ON DELETE CASCADE ON UPDATE CASCADE,
  UNIQUE ( WindowsID ),
  UNIQUE ( GmailID ),
  UNIQUE ( MicrosoftID ),
  UNIQUE ( SSNCode ),
  UNIQUE ( SSECode )
);
GRANT ALL ON MAlignments TO cmnoper;
GRANT SELECT, INSERT, UPDATE, DELETE ON MAlignments TO aploper;

DROP PROCEDURE UpsertAlignment;
CREATE PROCEDURE UpsertAlignment (
  pAccountID     UUID,
  iLicense       INTEGER,
  pWindowsID     VARCHAR(256),
  pGmailID       VARCHAR(256),
  pMicrosoftID   VARCHAR(256),
  pSequence      VARCHAR(6),
  pSSNCode       VARCHAR(24),
  pSSECode       VARCHAR(24)
) LANGUAGE SQL AS $$
  INSERT INTO MAlignments (AccountID, License, WindowsID, GmailID, MicrosoftID, Sequence, SSNCode, SSECode)
    VALUES (pAccountID, iLicense, pWindowsID, pGmailID, pMicrosoftID, pSequence, pSSNCode, pSSECode)
  ON CONFLICT ON CONSTRAINT malignments_pkey
  DO UPDATE SET License = iLicense, WindowsID = pWindowsID, GmailID = pGmailID, MicrosoftID = pMicrosoftID, Sequence = pSequence, SSNCode = pSSNCode, SSECode = pSSECode;
$$;




DROP PROCEDURE UpsertEmployAccount;
CREATE PROCEDURE UpsertEmployAccount (
  pOrgUnitCode  VARCHAR(8),
  pEmail        VARCHAR(256),
  pName         VARCHAR(256),
  pRead         VARCHAR(256),
  pStaffNumber  VARCHAR(5),
  pStaffType    INTEGER,
  pContractType INTEGER,
  iLicense      INTEGER,
  pWindowsID    VARCHAR(256),
  pGmailID      VARCHAR(256),
  pMicrosoftID  VARCHAR(256),
  pSequence     VARCHAR(6),
  pSSNCode      VARCHAR(24),
  pSSECode      VARCHAR(24)
) LANGUAGE 'plpgsql' AS $$
DECLARE
  pOrgUnitID     UUID;
  pAccountID     UUID;
BEGIN
  pOrgUnitID = GetOrgUnitID(pOrgUnitCode);
  CALL UpsertAccount(pEmail, pName, pRead, pOrgUnitID, 0);
  pAccountID = GetAccountIDByEmail(pEmail);
  CALL UpsertEmploy(pEmail, pStaffNumber, pStaffType, pContractType);
  CALL UpsertAlignment(pAccountID, iLicense, pWindowsID, pGmailID, pMicrosoftID, pSequence, pSSNCode, pSSECode);
END
$$;

CALL UpsertEmployAccount('00000000', '13001@class.bunri-s.ed.jp', '西野 哲也', 'にしの てつや', '00001', 0, 1, 1, '13001@class.bunri-s.ed.jp', '13001@class.bunri-s.ed.jp', '13001@class.bunri-s.ed.jp', '100001', '10000001', '12001011');
CALL UpsertEmployAccount('00000000', '1001@staff.arteria-s.net', '西野 哲也', 'にしの てつや', '00001', 0, 1, 1, '13001@class.bunri-s.ed.jp', '13001@class.bunri-s.ed.jp', '13001@class.bunri-s.ed.jp', '100001', '10000001', '12001011');

DROP PROCEDURE UpsertEmployAccountByName;
CREATE PROCEDURE UpsertEmployAccountByName (
  pOrgUnitCode          VARCHAR(8),
  pEmail                VARCHAR(256),
  pName                 VARCHAR(256),
  pRead                 VARCHAR(256),
  pStaffNumber          VARCHAR(5),
  pStaffTypeName        VARCHAR(16),
  pContractTypeName     VARCHAR(16),
  iLicense              INTEGER,
  pWindowsID            VARCHAR(256),
  pGmailID              VARCHAR(256),
  pMicrosoftID          VARCHAR(256),
  pSequence             VARCHAR(6),
  pSSNCode              VARCHAR(24),
  pSSECode              VARCHAR(24)
) LANGUAGE 'plpgsql' AS $$
DECLARE
  pStaffTypeID      INTEGER;
  pContractTypeID   INTEGER;
BEGIN
  SELECT GetStaffTypeCode(pStaffTypeName) INTO pStaffTypeID;
  SELECT GetContractTypeCode(pContractTypeName) INTO pContractTypeID;
  CALL UpsertEmployAccount(pOrgUnitCode, pEmail, pName, pRead, pStaffNumber, pStaffTypeID, pContractTypeID, iLicense, pWindowsID, pGmailID, pMicrosoftID, pSequence, pSSNCode, pSSECode);
END
$$;

CALL UpsertEmployAccountByName('00000000', '9001@staff.arteria-s.net', '西野 哲也', 'にしの てつや', '00001', '教育職員', '非常勤', 1, '13001@class.bunri-s.ed.jp', '13001@class.bunri-s.ed.jp', '13001@class.bunri-s.ed.jp', '100001', '910000001', '912001011');


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
GRANT SELECT, INSERT, UPDATE, DELETE ON MGroups TO aploper;

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
GRANT SELECT, INSERT, UPDATE, DELETE ON MMembers TO aploper;

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
