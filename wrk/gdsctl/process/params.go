package process

import (
	"encoding/json"
	"errors"
	"fmt"
	"io"
	"os"
)

// GlobalParams グローバルパラメータ
type GlobalParams struct {
	DbsServeName    string `json:"dbsServeName"`    //　データベースサーバ名
	DatabaseName    string `json:"databaseName"`    //　データベース名
	DocumentRoot    string `json:"documentRoot"`    //　ドキュメントルートフォルダ
	AllowOriginURL  string `json:"allowOriginURL"`  //　API サーバにアクセスを許可するURL
	AssociateDomain string `json:"AssociateDomain"` //　所属ドメイン名（クッキーに含めるドメイン名）
	CrtFilepath     string `json:"crtFilepath"`     //　デジタル証明書ファイルパス
	KeyFilepath     string `json:"keyFilepath"`     //　秘密鍵ファイルパス
	CredentialFile  string `json:"credentialFile"`  //　クレデンシャルファイルパス
	ImpersonateUser string `json:"impersonateUser"` //　代行ユーザー識別子
}

// LoadParams グローバルパラメータを入力
func (context *Context) LoadParams(filepath string, globalParams *GlobalParams) error {
	fmt.Printf("グローバルパラメータを入力します。[%s]\n", filepath)
	pFile, pError := os.Open(filepath)
	if pError != nil {
		return pError
	}
	defer pFile.Close()
	if pError == nil {
		pBytes, pError := io.ReadAll(pFile)
		if pError == nil {
			fmt.Println(string(pBytes))
			json.Unmarshal(pBytes, globalParams)
		}
	}

	return nil
}

// LoadParams グローバルパラメータを入力
func LoadParams(filepath string, globalParams *GlobalParams) error {
	pFile, pError := os.Open(filepath)
	if pError != nil {
		return pError
	}
	defer pFile.Close()

	pBytes, pError := io.ReadAll(pFile)
	if pError != nil {
		return fmt.Errorf("failure ReadAll: %v", pError)
	}
	fmt.Println(string(pBytes))

	pError = json.Unmarshal(pBytes, globalParams)
	if pError != nil {
		return fmt.Errorf("failed Unmarshal: %v", pError)
	}

	return nil
}

// SaveParams グローバルパラメータを保存
func (context *Context) SaveParams(filepath string, pParams *GlobalParams) error {
	pFile, pError := os.Create(filepath)
	if pError != nil {
		return fmt.Errorf("failed Marshal: %v", pError)
	}
	defer pFile.Close()

	pBytes, pError := json.Marshal(pParams)
	if pError != nil {
		return fmt.Errorf("failed Marshal: %v", pError)
	}

	nBytes, pError := pFile.Write(pBytes)
	if pError != nil {
		return fmt.Errorf("failed Write: %v", pError)
	}
	if nBytes != len(pBytes) {
		pError := errors.New("failed Write, loss bytes")
		return pError
	}

	return nil
}

// パラメータをダンプ
func (pData *GlobalParams) Dump() {
	fmt.Println("[Global Parameters]")
	fmt.Println("DbsServeName: ", pData.DbsServeName)
	fmt.Println("DatabaseName: ", pData.DatabaseName)
	fmt.Println("DocumentRoot: ", pData.DocumentRoot)
	fmt.Println("AllowOriginURL: ", pData.AllowOriginURL)
	fmt.Println("AssociateDomain: ", pData.AssociateDomain)
	fmt.Println("CrtFilepath: ", pData.CrtFilepath)
	fmt.Println("KeyFilepath: ", pData.KeyFilepath)
	fmt.Println("CredentialFile: ", pData.CredentialFile)
	fmt.Println("ImpersonateUser: ", pData.ImpersonateUser)
}
