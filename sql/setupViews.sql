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
CREATE VIEW VEmploys AS
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
  MEmploys.StaffNumber,
  MEmploys.StaffType,
  MEmploys.ContractType
FROM
    MAccounts
      INNER JOIN MEmploys ON MAccounts.AccountID = MEmploys.AccountID;
GRANT SELECT ON VEmploys TO aploper, cmnoper;
