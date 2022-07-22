#IT-story API

 - [IT-story API](#it-story-api)
     - [Auth](#authentification)
        - [Register](#register)
            - [Register Request](#register-request-body)
            - [Register Response](#register-response)
        - [Login](#login)
            - [Login Request](#login-request-body)
            - [Login Response](#login-response)

###Authentification

####Register
```js
POST {{host}}\api\auth\register
```

######Register request body
```json
{
    "username": "AlexSmith",
    "email": "test@email.com",
    "password": "testPassword",
    "confirmPassword": "testPassword",
    "firstName": "Alex",
    "lastName": "Smith",
    "status": "student"
}
```
######Register Response

```json
200 OK
```

```json
{
    "id": "1",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6Ikp",
    "username": "AlexSmith",
    "email": "test@email.com",
    "firstName": "Alex",
    "lastName": "Smith",
    "status": "student"
}
```
####Login
```js
POST {{host}}\api\auth\login
```

######Login request body
```json
{
    "email": "test@email.com",
    "password": "testPassword",
}
```
######Login Response

```json
200 OK
```

```json
{
    "id": "1",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6Ikp",
    "username": "AlexSmith",
    "email": "test@email.com",
    "firstName": "Alex",
    "lastName": "Smith",
    "status": "student"
}
```