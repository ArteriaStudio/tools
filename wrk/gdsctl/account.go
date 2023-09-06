package main

import (
	"context"
	"fmt"
	"os"

	"golang.org/x/oauth2/google"
	admin "google.golang.org/api/admin/directory/v1"
	"google.golang.org/api/option"
)

// Build and returns an Admin SDK Directory service object authorized with
// the service accounts that act on behalf of the given user.
// Args:
//
//	user_email: The email of the user. Needs permissions to access the Admin APIs.
//
// Returns:
//
//	Admin SDK directory service object.
func CreateDirectoryService(userEmail string, ServiceAccountFilePath string) (*admin.Service, error) {
	pContext := context.Background()

	jsonCredentials, pError := os.ReadFile(ServiceAccountFilePath)
	if pError != nil {
		return nil, pError
	}

	pConfig, pError := google.JWTConfigFromJSON(jsonCredentials, admin.AdminDirectoryUserScope)
	if pError != nil {
		return nil, fmt.Errorf("JWTConfigFromJSON: %v", pError)
	}
	pConfig.Subject = userEmail

	pTokenSource := pConfig.TokenSource(pContext)

	pService, pError := admin.NewService(pContext, option.WithTokenSource(pTokenSource))
	if pError != nil {
		return nil, fmt.Errorf("NewService: %v", pError)
	}

	return pService, nil
}
