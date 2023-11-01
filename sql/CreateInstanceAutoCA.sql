CREATE DATABASE AutoCA WITH OWNER = bkowner;
\c autoca bkowner

CREATE SEQUENCE SQ_CERTS;
GRANT USAGE ON SEQUENCE SQ_CERTS TO aploper

/* 発行した証明書 */
DROP TABLE TIssuedCerts;
CREATE TABLE TIssuedCerts (
  SequenceNumber INTEGER NOT NULL,
  SerialNumber   VARCHAR(48) NOT NULL,
  CommonName     VARCHAR(256) NOT NULL,
  CA             BOOLEAN NOT NULL,
  Revoked        BOOLEAN NOT NULL,
  LaunchAt       TIMESTAMP NOT NULL,
  ExpireAt       TIMESTAMP NOT NULL,
  PemData        TEXT NOT NULL,
  KeyData        TEXT NULL,
  PRIMARY KEY (SequenceNumber),
  UNIQUE (SerialNumber)
);
GRANT SELECT, UPDATE, INSERT, DELETE ON TIssuedCerts TO aploper;

/* 組織プロファイル */
DROP TABLE TOrgProfile;
CREATE TABLE TOrgProfile (
  OrgKey        INTEGER      NOT NULL,
  OrgName       VARCHAR(64)  NOT NULL,
  OrgUnitName   VARCHAR(48)  NOT NULL,
  LocalityName  VARCHAR(32)  NOT NULL,
  ProvinceName  VARCHAR(32)  NOT NULL,
  CountryName   VARCHAR(2)   NOT NULL,
  ServerName    VARCHAR(256) NOT NULL,
  UpdateAt      TIMESTAMP    NOT NULL,
  PRIMARY KEY (OrgKey)
);
GRANT SELECT, UPDATE , INSERT, DELETE ON TOrgProfile TO aploper;

/* 認証局主体者情報 */
DROP TABLE TAuthority;
CREATE TABLE TAuthority (
  AuthorityKey  INTEGER NOT NULL,
  AuthorityName VARCHAR(16) NOT NULL,
  PRIMARY KEY (AuthorityKey),
  FOREIGN KEY (AuthorityKey) REFERENCES TOrgProfile (OrgKey)
);
GRANT SELECT, UPDATE , INSERT, DELETE ON TAuthority TO aploper;
INSERT INTO TAuthority VALUES (0, 'ARTERIAS');
