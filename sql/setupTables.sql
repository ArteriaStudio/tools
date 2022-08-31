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


