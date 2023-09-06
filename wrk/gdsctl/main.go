package main

import (
	"encoding/base64"
	"fmt"
	"os"

	"arteria-s.net/process"
	admin "google.golang.org/api/admin/directory/v1"
)

var pVersion string = "gsctl Version 0.0.0"

// globalParams グローバルパラメータ
var globalParams process.GlobalParams

func main() {
	//　バージョン表示
	fmt.Printf("%s\n", pVersion)

	//　プロセスを初期化
	pProcess := process.Initialize()
	pProcess.PrepareEnviroments()

	//　グローバルパラメータファイルを入力
	pConfFile := pProcess.HomeDir + "/config/" + pProcess.Hostname + ".json"
	fmt.Println("Configuration Parameter File: " + pConfFile)
	pProcess.LoadParams(pConfFile, &globalParams)
	//	pProcess.SaveParams(pConfFile, &globalParams)
	globalParams.Dump()

	//　サービスアカウントクレデンシャルファイル
	pCredentialFile := pProcess.HomeDir + "/" + globalParams.CredentialFile
	fmt.Printf("Credential File: %s", pCredentialFile)

	//　サービスアカウントを指定
	os.Setenv("GOOGLE_APPLICATION_CREDENTIALS", pCredentialFile)

	//　サービスに接続
	pService, pError := CreateDirectoryService(globalParams.ImpersonateUser, pCredentialFile)
	if pError != nil {
		return
	}

	//　Workspace Directoryのユーザーを列挙
	DoListupUsers(pService)
}

// 鍵を生成
func DoCreateKey(pKeyFilepath string, pKeyIdentityName string) int {
	// サービスアカウントの鍵を生成
	pKey, pError := CreateKey(os.Stdout, pKeyIdentityName)
	if pError != nil {
		fmt.Print(pError)
		return (-1)
	}

	// 鍵をファイルに保存
	pBytes, _ := base64.StdEncoding.DecodeString(pKey.PrivateKeyData)
	WriteToFile(pKeyFilepath, pBytes)

	return (0)
}

// 鍵を一覧
func DoListupKey(pKeyIdentityName string) int {
	// サービスアカウントの鍵を一覧
	pKeys, pError := ListKeys(os.Stdout, pKeyIdentityName)
	if pError != nil {
		fmt.Print(pError)
		return (-1)
	}
	fmt.Println("")
	for iKey, pKey := range pKeys {
		fmt.Printf("KeyName[%d]: %s\n", iKey, pKey.Name)
	}

	return (0)
}

// DoListupUsers Workspaceユーザーを一覧
func DoListupUsers(pService *admin.Service) error {
	pResult, pError := pService.Users.List().Customer("my_customer").MaxResults(10).
		OrderBy("email").Do()
	if pError != nil {
		return fmt.Errorf("unable to retrieve users in domain: %v", pError)
	}

	if len(pResult.Users) == 0 {
		return fmt.Errorf("users not found: %v", pError)
	} else {
		fmt.Print("Users:\n")
		for _, u := range pResult.Users {
			fmt.Printf("%s (%s)\n", u.PrimaryEmail, u.Name.FullName)
		}
	}

	return nil
}
