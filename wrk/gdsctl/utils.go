package main

import (
	"errors"
	"os"
)

// 指定のファイルをバイト列を保存
func WriteToFile(pFilepath string, pBytes []byte) error {
	pFile, pError := os.Create(pFilepath)
	if pError != nil {
		return (pError)
	}
	nBytes, pError := pFile.Write((pBytes))
	if pError != nil {
		return (pError)
	}
	if nBytes != len(pBytes) {
		return (errors.New("WriteToFile: unmatch write number of bytes"))
	}

	return pFile.Close()
}
