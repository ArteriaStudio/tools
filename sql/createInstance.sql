/*　状況確認　*/
/*
select * from pg_extension;
select * from pg_available_extensions;
*/

/*　ロールを登録　*/
/*（整理）*/
/*　bkowner：テーブルやビューのオーナー。システムカタログとの区別に用いる　*/
/*　aploper：アプリケーション向け窓口　*/
/*　cmnoper：共通データの保護　*/
/*　archons：利用者認証　*/
CREATE ROLE bkowner WITH LOGIN;
CREATE ROLE aploper WITH LOGIN;
CREATE ROLE cmnoper WITH LOGIN;
CREATE ROLE archons WITH LOGIN;

/*　データベースを生成　*/
DROP DATABASE sbook;
CREATE DATABASE sbook WITH OWNER = bkowner;

/*　拡張機能を生成　*/
CREATE EXTENSION "uuid-ossp";
CREATE EXTENSION "pgcrypto";
