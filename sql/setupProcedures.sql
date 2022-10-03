
/*　生徒情報を更新（年度毎処理）　*/
CREATE PROCEDURE InsertStudent (pYear INTEGER, pSchool VARCHAR(3), pGrade VARCHAR(1), pName VARCHAR(256), pRead VARCHAR(256), pSets VARCHAR(2), pNumbers VARCHAR(2), pGender VARCHAR(1), pBirthAt TIMESTAMP, pStudentNumber VARCHAR(5))
LANGUAGE SQL AS $$
DECLARE
  pEmail        VARCHAR(256);
  pAccountID    UUID;
BEGIN
  pEmail := GenerateEmailForStudent(pStudentNumber);
  CALL UpsertAccount(pEmail, pName, pRead, pOrgUnit, 0);
  
  INSERT INTO MStudents (StudentNumber, AccountID, Gender, BirthAt) (SELECT '00000', AccountID, '女', now() FROM MAccounts WHERE Email = 'wakaba@arteria-s.net');
END;
$$;

