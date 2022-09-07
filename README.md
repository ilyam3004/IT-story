# IT-story API

- [IT-story API](#it-story-api)
  - [Auth](#authentification)
    - [Register](#register)
    - [Login](#login)
  - [Posts](#posts)
    - [Add posts](#add-posts)
    - [Edit posts](#edit-posts)
    - [Remove posts](#remove-posts)
    - [Like posts](#like-posts)
    - [Unlike posts](#unlike-posts)
    - [Get liked posts](#liked-posts)
    - [Comment post](#comment-post)
    - [Remove comment](#remove-comment)
    - [Reply comment](#reply-comment)
    - [Remove reply](#remove-reply)
    - [Save post to bookmarks](#save-post-to-bookmarks)
    - [Remove post from bookmarks](#remove-post-from-bookmarks)
    - [Get bookmarks](#get-bookmarks)
  - [Followings](#followings)
    - [Get followers](#get-followers)
    - [Get followings](#get-followings)
    - [Follow user](#follow-user)
    - [Unfollow user](#unfollow-user)
    

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

[⬆Back to navigation](#it-story-api)
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
[⬆Back to navigation](#it-story-api)

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
[⬆Back to navigation](#it-story-api)
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
[⬆Back to navigation](#it-story-api)

### Remove posts

If you want to remove posts you need login and make this request.

```js
DELETE {{host}}/posts/removepost/{postId}
```

#### Request headers

```js
Authorization: your_token;
```

#### Response
It will return 200 OK if post was removed.
```js
200 OK
```
and this message 
```json
Post successfully removed from your posts
```
or 404 if post was not found.

```js
404 Not Found
```
```json
{
    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
    "title": "Post not found",
    "status": 404,
    "traceId": "00-eb8b754c730f4594ee822b8a3479888d-52c96af4b54c61cb-00"
}
```
[⬆Back to navigation](#it-story-api)
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
409 Conflict
```

```js
{
    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.8",
    "title": "You already liked this post",
    "status": 409,
    "traceId": "00-921aa53f0ab828d52bd5a362737b7f11-b84ab3e22c5d7815-00"
}
```
[⬆Back to navigation](#it-story-api)
### Unlike posts

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
409 Conflict
```

```js
{
    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.8",
    "title": "This post wasn't liked by you.",
    "status": 409,
    "traceId": "00-0f36ba4ed7221f2a9ae018aeaeb54a3c-1ec3a53ac6adf0d4-00"
}
```
[⬆Back to navigation](#it-story-api)
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
404 Not Found
```
```json
{
    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
    "title": "You didn't like any post",
    "status": 404,
    "traceId": "00-c427120473272a8709b17eb6e8ec175c-c7fad1e165f4dcd5-00"
}
```
[⬆Back to navigation](#it-story-api)
### Comment post

To comment on posts you need login and make this request.

```js
POST {{host}}/posts/commentpost
```

#### Request headers

```js
Authorization: your_token;
```

#### Request body
```json
{
  "postId" : 1,
  "text" : "Comment to post with id 1"
}
```

#### Response
It will return 200 OK and post with new comment
```js
200 OK
```
```json
{
    "id": 25,
    "userId": 14,
    "text": "some new super super text of my post",
    "date": "2022-08-29T19:33:36.0953429Z",
    "comments": [
        {
            "id": 2,
            "postId": 25,
            "userId": 14,
            "text": "comment to my post",
            "date": "2022-08-30T07:39:52.9193615Z",
            "replies": []
        }
    ],
    "likes": 3
}
```
[⬆Back to navigation](#it-story-api)

### Remove comment

To remove comment from posts you need login and make this request.

```js
DELETE {{host}}/posts/removeComment/{commentId}
```

#### Request headers

```js
Authorization: your_token;
```

#### Response
It will return 200 OK and message if comment was successfully removed.
```js
200 OK
```
```json
Comment successfully removed
```
or 404 if comment wasn't found.
```js
404 Not Found
```
```json
{
    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
    "title": "Comment not found",
    "status": 404,
    "traceId": "00-6c08b391bbe1ea565e8fc65e938c6a0d-61e9d3c90048baf2-00"
}
```
[⬆Back to navigation](#it-story-api)
### Reply comment

To reply comment you need login and make this request.

```js
POST {{host}}/posts/reply
```

#### Request headers

```js
Authorization: your_token;
```
#### Request body
```json
{
  "commentId" : 1,
  "userId" : 14,
  "text" : "Reply to comment with userId 14 and id 1"
}
```

#### Response
It will return 200 OK and post with new reply
```js
200 OK
```
```json
Comment successfully removed
```
or 404 if comment wasn't found.
```js
404 Not Found
```
```json
{
    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
    "title": "Comment not found",
    "status": 404,
    "traceId": "00-6c08b391bbe1ea565e8fc65e938c6a0d-61e9d3c90048baf2-00"
}
```
[⬆Back to navigation](#it-story-api)

### Remove reply

To remove reply from comment you need login and make this request.

```js
DELETE {{host}}/posts/removereply/{replyId}
```

#### Request headers

```js
Authorization: your_token;
```

#### Response
It will return 200 OK and message if reply was successfully removed.
```js
200 OK
```
```json
Reply successfully removed
```
or 404 if reply wasn't found.

```json
{
    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
    "title": "Reply not found",
    "status": 404,
    "traceId": "00-6c08b391bbe1ea565e8fc65e938c6a0d-61e9d3c90048baf2-00"
}
```
[⬆Back to navigation](#it-story-api)
### Save post to bookmarks

To save post to bookmarks you need to make this one.

```js
POST {{host}}/posts/savepost/{postId}
```

#### Request headers

```js
Authorization: your_token;
```

#### Response
It will return 200 OK and your new bookmark
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
or 404 if post wasn't found.
```js
404 Not Found
```
```json
{
    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
    "title": "Post not found",
    "status": 404,
    "traceId": "00-07ca577eb1fb2ad5325eee3ab40a47a9-310a987dc7e16b45-00"
}
```
[⬆Back to navigation](#it-story-api)
### Remove post from bookmarks

To remove post from your bookmarks you need to make this one.

```js
DELETE {{host}}/posts/unsavepost/{postId}
```

#### Request headers

```js
Authorization: your_token;
```

#### Response
It will return 200 OK
```js
200 OK
```
and this message if post was successfully removed from bookmarks
```json
Post successfully removed from your bookmarks
```
or 404 if post wasn't found.
```js
404 Not Found
```
```json
{
    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
    "title": "Saved post not found",
    "status": 404,
    "traceId": "00-b69d4d5be7afd34860d1b23401cc5155-379626810cf1c56d-00"
}
```
[⬆Back to navigation](#it-story-api)
### Get Bookmarks


If you want te get your bookmarks make this request.

```js
GET {{host}}/posts/bookmarks
```

#### Request headers

```js
Authorization: your_token;
```

#### Response
It will return 200 OK and all your bookmarks
```js
200 OK
```
```json
[
    {
        "id": 23,
        "userId": 13,
        "text": "some text of my post",
        "date": "2022-08-29T08:28:06.4234939Z",
        "comments": [],
        "likes": 1
    },
    {
        "id": 24,
        "userId": 13,
        "text": "New text of my post??",
        "date": "2022-08-29T19:23:12.4257879Z",
        "comments": [],
        "likes": 1
    },
    {
        "id": 25,
        "userId": 14,
        "text": "some new super super text of my post",
        "date": "2022-08-29T19:33:36.0953429Z",
        "comments": [],
        "likes": 3
    }
]
```
or 404 if post wasn't found.
```js
404 Not Found
```
```json
{
    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
    "title": "Saved posts not found",
    "status": 404,
    "traceId": "00-6b8fa3b3ba57dfa8cc2c343429963080-3b73e0e736efaba2-00"
}
```
[⬆Back to navigation](#it-story-api)
## Following system

### Get followers

To watch all your followers you need to make this request.

```js
GET {{host}}/followers
```

#### Request headers

```js
Authorization: your_token;
```

#### Response
It will return 200 OK and all your followers
```js
200 OK
```

```json
[
    {
        "id": 15,
        "email": "test@gmail.com",
        "username": "testusername",
        "firstName": "testfirstname",
        "lastName": "testlastname",
        "status": "student"
    },
    {
        "id": 15,
        "email": "testemail@gmail.com",
        "username": "test",
        "firstName": "test",
        "lastName": "test",
        "status": "student"
    }
]
```
or 404 if followers wasn't found.
```js
404 Not Found
```
```json
{
    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
    "title": "Followers not found",
    "status": 404,
    "traceId": "00-a547e12703e343860ec340131f8df0e8-ededeae2cfbc931c-00"
}
```
[⬆Back to navigation](#it-story-api)
### Get followings

To watch all your followings you need to make this request.

```js
GET {{host}}/followings
```

#### Request headers

```js
Authorization: your_token;
```

#### Response
It will return 200 OK and all your followings
```js
200 OK
```

```json
[
    {
        "id": 13,
        "email": "ilya@gmail.com",
        "username": "Ilya_3004",
        "firstName": "Ilya",
        "lastName": "Melnichuk",
        "status": "student"
    },
    {
        "id": 12,
        "email": "ilyamelnichuk3004@gmail.com",
        "username": "Ilya_3004",
        "firstName": "Ilya",
        "lastName": "Melnichuk",
        "status": "student"
    }
]
```
or 404 if followings wasn't found.

```js
404 Not Found
```

```json
{
    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
    "title": "Followings not found",
    "status": 404,
    "traceId": "00-aed80dccb239a34590fb5584c0c42061-36bf620ff07ad344-00"
}
```
[⬆Back to navigation](#it-story-api)
### Follow user

To follow someone make this request.

```js
POST {{host}}/follow/{userId}
```

#### Request headers

```js
Authorization: your_token;
```

#### Response
It will return 200 OK
```js
200 OK
```

```json
{
  "id": 6,
  "followingId": 13,
  "followerId": 14
}
```
or 404 if user would you want to follow wasn't found.

```js
404 Not Found
```

```json
{
    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
    "title": "User would you want to follow not found",
    "status": 404,
    "traceId": "00-4dc7c869cd7b580e750a2ea113a930f9-6fc1e60b8e3293b0-00"
}
```
[⬆Back to navigation](#it-story-api)
### Unfollow user

To unfollow someone make this request.

```js
POST {{host}}/unfollow/{userId}
```

#### Request headers

```js
Authorization: your_token;
```

#### Response
It will return 200 OK
```js
200 OK
```

```json

```
or 404 if user would you want to unfollow wasn't found.

```js
404 Not Found
```

```json
{
    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
    "title": "User would you want to unfollow not found",
    "status": 404,
    "traceId": "00-5d6297227f01ca7e24209aed794aa8eb-54952409e40abf01-00"
}
```
[⬆Back to navigation](#it-story-api)

