package main

import (
	"context"
	// "encoding/base64"
	"fmt"
	"io"

	iam "google.golang.org/api/iam/v1"
)

// createKey creates a service account key.
func CreateKey(w io.Writer, serviceAccountEmail string) (*iam.ServiceAccountKey, error) {
	ctx := context.Background()
	service, err := iam.NewService(ctx)
	if err != nil {
		return nil, fmt.Errorf("iam.NewService: %v", err)
	}

	resource := "projects/-/serviceAccounts/" + serviceAccountEmail
	request := &iam.CreateServiceAccountKeyRequest{}
	key, err := service.Projects.ServiceAccounts.Keys.Create(resource, request).Do()
	if err != nil {
		return nil, fmt.Errorf("Projects.ServiceAccounts.Keys.Create: %v", err)
	}
	// The PrivateKeyData field contains the base64-encoded service account key
	// in JSON format.
	// TODO(Developer): Save the below key (jsonKeyFile) to a secure location.
	// You cannot download it later.
	// jsonKeyFile, _ := base64.StdEncoding.DecodeString(key.PrivateKeyData)
	fmt.Fprintf(w, "Key created successfully")
	return key, nil
}

// listKey lists a service account's keys.
func ListKeys(w io.Writer, serviceAccountEmail string) ([]*iam.ServiceAccountKey, error) {
	ctx := context.Background()
	service, err := iam.NewService(ctx)
	if err != nil {
		return nil, fmt.Errorf("iam.NewService: %v", err)
	}

	resource := "projects/-/serviceAccounts/" + serviceAccountEmail
	response, err := service.Projects.ServiceAccounts.Keys.List(resource).Do()
	if err != nil {
		return nil, fmt.Errorf("Projects.ServiceAccounts.Keys.List: %v", err)
	}
	for _, key := range response.Keys {
		fmt.Fprintf(w, "Listing key: %v", key.Name)
	}
	return response.Keys, nil
}
