package process

import (
	"flag"
	"fmt"
	"os"
)

// Context プロセスコンテキスト
type Context struct {
	HomeDir  string //　プロセス作業ディレクトリ
	Hostname string //　API サーバを実行しているサーバのホスト名
}

// Initialize プロセスインスタンスを初期化
func Initialize() (context *Context) {
	context = new(Context)
	return (context)
}

// PrepareEnviroments 起動引数と環境変数を入力
func (context *Context) PrepareEnviroments() {
	//　起動引数を入力
	flag.Parse()
	nArgs := flag.NArg()
	pArgs := flag.Args()
	fmt.Println(pArgs)

	if nArgs < 1 {
		fmt.Println("起動引数に必須引数がありません。作業ディレクトリを指定してください。")
		fmt.Println("Usage: $(PROG) $(WorkDirectory}")
		os.Exit((-1))
		return
	}

	context.HomeDir = pArgs[0]

	hostname, err := os.Hostname()
	if err != nil {
		fmt.Println("ホスト名を獲得できませんでした。")
		return
	}
	context.Hostname = hostname
}
