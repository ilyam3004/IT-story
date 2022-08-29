# IT-story API

- [IT-story API](#it-story-api)
  - [Auth](#authentification)
    - [Register](#register)
      - [Register Request](#register-request-body)
      - [Register Response](#register-response)
    - [Login](#login)
      - [Login Request](#login-request-body)
      - [Login Response](#login-response)
    - [Posts](#posts)
    

## Authentification
First what we need to do is to create an account.

### Register

```js
POST {{host}}/auth/register
```
#### Register request body

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
#### Register Response
```js
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

After registration we can login to our account.
### Login

```js
POST {{host}}/auth/login
```

#### Login request body

```json
{
  "email": "test@email.com",
  "password": "testPassword"
}
```

#### Login Response

```js
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

## Posts

### Add posts

To add posts you need login and make this request.

```js
POST {{host}}/posts/addpost
```

#### Request headers

```js
Authorization: your_token;
```

#### Request body
```json
{
  "string": "text"
}
```

#### Response

```js
200 OK
```

```json
{
  "id": 23,
  "userId": 13,
  "text": "some text of my post",
  "date": "2022-08-29T08:28:06.4234939Z",
  "comments": [],
  "likes": 0
}
```

### Edit posts

If you want to edit posts you need login and make this request.

```js
PUT {{host}}/posts/editpost
```

#### Request headers

```js
Authorization: your_token;
```

#### Request body
```json
{
  "postId" : 1,
  "newtext" : "new text of my post😊"
}
```

#### Response

```js
200 OK
```

```json
{
  "id": 1,
  "userId": 13,
  "text": "new text of my post😊",
  "date": "2022-08-29T08:28:06.4234939Z",
  "comments": [],
  "likes": 124
}
```


### Like posts

To like posts you need login and make this one.

```js
POST {{host}}/posts/likepost/{postId}
```

#### Request headers

```js
Authorization: your_token;
```

#### Response
It will return 200 OK and post with incremented count of likes if you like post 
```js
200 OK
```

```json
{
  "id": 23,
  "userId": 13,
  "text": "some text of my post",
  "date": "2022-08-29T08:28:06.4234939Z",
  "comments": [],
  "likes": 1
}
```

or 409 if you already liked it.

```js
409 OK
```

```js
{
    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.8",
    "title": "You already liked this post",
    "status": 409,
    "traceId": "00-921aa53f0ab828d52bd5a362737b7f11-b84ab3e22c5d7815-00"
}
```

### UnLike posts

To remove like from posts you need login and make this one.

```js
POST {{host}}/posts/unlikepost/{postId}
```

#### Request headers

```js
Authorization: your_token;
```

#### Response
It will return 200 OK and post with decremented count of likes if you unlike post 
```js
200 OK
```

```json
{
  "id": 23,
  "userId": 13,
  "text": "some text of my post",
  "date": "2022-08-29T08:28:06.4234939Z",
  "comments": [],
  "likes": 0
}
```

or 409 if you don't like this post.

```js
409 OK
```

```js
{
    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.8",
    "title": "This post wasn't liked by you.",
    "status": 409,
    "traceId": "00-0f36ba4ed7221f2a9ae018aeaeb54a3c-1ec3a53ac6adf0d4-00"
}
```

### Liked posts

To get all posts you liked make this request.

```js
GET {{host}}/posts/likedposts
```

#### Request headers

```js
Authorization: your_token;
```

#### Response
It will return 200 OK if you have liked any posts. 
```js
200 OK
```

```json
[
    {
        "id": 1,
        "userId": 14,
        "text": "some new super super text of my post",
        "date": "2022-08-29T19:33:36.0953429Z",
        "comments": [],
        "likes": 14
    },
    {
        "id": 2,
        "userId": 14,
        "text": "some new super super text of my post",
        "date": "2022-08-29T19:33:36.0953429Z",
        "comments": [],
        "likes": 23
    },
    {
        "id": 3,
        "userId": 14,
        "text": "some new super super text of my post",
        "date": "2022-08-29T19:33:36.0953429Z",
        "comments": [],
        "likes": 12
    }
]
```
or 404 if you haven't liked any posts.
```js
404 OK
```
```json
{
    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
    "title": "You didn't like any post",
    "status": 404,
    "traceId": "00-c427120473272a8709b17eb6e8ec175c-c7fad1e165f4dcd5-00"
}
```







