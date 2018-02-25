# .NET core WebApi project and RavenDB


---

## Get user list

### Request

**URL:** `http://localhost:5000/api/users/getlist`

**Method:** `GET`

### Response

**Content-Type:** `application/json; charset=utf-8`

**Status code:** `200`

**Body:**
```
[
    "users-1-A",
    "users-2-A",
    "users-3-A"
]
```

---

## Get user info by id

### Request

**URL:** `http://localhost:5000/api/users/getinfo?id=users-1-A`

**Method:** `GET`

### Response

**Content-Type:** `application/json; charset=utf-8`

**Status code:** `200`

**Body:**
```
{
    "id": "users-1-A",
    "name": "John",
    "age": 19
}
```

---

## Create new user

### Request

**URL:** `http://localhost:5000/api/users/create`

**Method:** `POST`

**Headers:**

- `Content-Type` : `application/json`

**Body:**
```
{
	"Name": "Zoe",
	"Age": 33
}
```

### Response

**Content-Type:** `application/json; charset=utf-8`

**Status code:** `201`

**Body:**
```
{
    "id": "users-4-A",
    "name": "Zoe",
    "age": 33
}
```

---

## Delete user

### Request

**URL:** `http://localhost:5000/api/users/delete?id=users-1-A`

**Method:** `DELETE`

### Response

**Status code:** `200`

---

## Edit user

### Request

**URL:** `http://localhost:5000/api/users/edit?id=users-4-A`

**Method:** `PUT`

**Headers:**

- `Content-Type` : `application/json`

**Body:**
```
{
	"Name": "Zoe",
	"Age": 20
}
```

### Response

**Status code:** `200`

---
