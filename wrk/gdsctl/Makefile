PROG=gsctl
SRCS=main.go account.go key.go util.go
OBJS=$(SRCS:.go=.o)

$(PROG): $(SRCS)
	go build -o $(PROG) $(SRCS)
