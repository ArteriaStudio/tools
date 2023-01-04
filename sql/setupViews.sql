/*　生徒一覧　*/
CREATE VIEW VStudents AS
SELECT
  MAccounts.AccountID,
  MAccounts.Email,
  MAccounts.Name,
  MAccounts.Read,
  MAccounts.OrgUnitID,
  MAccounts.Status,
  MAccounts.ExpireAt,
  MAccounts.UpdateAt,
  MAccounts.DeleteAt,
  MStudents.StudentNumber,
  MStudents.Gender,
  MStudents.BirthAt,
  MStudents.EnterAt,
  MStudents.LeaveAt,
  MGrade.Year,
  MGrade.School,
  MGrade.Grade,
  MGrade.Sets,
  MGrade.Numbers
  FROM
    MAccounts
      INNER JOIN MGrade ON MAccounts.AccountID = MGrade.AccountID
      INNER JOIN MStudents ON MAccounts.AccountID = MStudents.AccountID;
GRANT SELECT ON VStudents TO aploper, cmnoper;


/*　職員一覧　*/
DROP VIEW VEmploys;
CREATE VIEW VEmploys AS
SELECT
  MAccounts.AccountID,
  MAccounts.Email,
  MAccounts.Name,
  MAccounts.Read,
  MAccounts.OrgUnitID,
  MOrgUnits.Name AS OrgUnitName,
  MAccounts.Status,
  MAccounts.ExpireAt,
  MAccounts.UpdateAt,
  MAccounts.DeleteAt,
  MEmploys.StaffNumber,
  MEmploys.StaffType,
  MStaffTypes.Text AS StaffTypeName,
  MEmploys.ContractType,
  MContractTypes.Text AS ContractTypeName
FROM
  MAccounts
    INNER JOIN MEmploys ON MAccounts.AccountID = MEmploys.AccountID
    INNER JOIN MOrgUnits ON MAccounts.OrgUnitID = MOrgUnits.OrgUnitID
    INNER JOIN MStaffTypes ON MEmploys.StaffType = MStaffTypes.StaffType
    INNER JOIN MContractTypes ON MEmploys.ContractType = MContractTypes.ContractType
;
GRANT SELECT ON VEmploys TO aploper, cmnoper;

/*　組織単位　*/
DROP VIEW VOrgUnits;
CREATE VIEW VOrgUnits AS
SELECT
  MOrgUnits.OrgUnitID,
  MOrgUnits.Code,
  MOrgUnits.Name,
  MOrgRels.Year,
  MOrgRels.ContainerID
FROM
  MOrgUnits
    LEFT OUTER JOIN MOrgRels ON MOrgRels.OrgUnitID = MOrgUnits.OrgUnitID
ORDER BY
  ContainerID, Code;
GRANT SELECT ON VOrgUnits TO aploper, cmnoper;

