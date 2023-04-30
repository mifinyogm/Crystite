# Worlds
Contains routes and objects for interacting with NeosVR worlds, such as loading,
focusing, saving, closing or modifying them.

## Objects
### RestWorld
A subset of fields from Neos's `World` object.

| Field       | Type   | Description                  |
|-------------|--------|------------------------------|
| id          | string | the session ID of the world  |
| name        | string | the name of the world        |
| description | string | the description of the world |

#### Examples
```json
{
  "id": "s-31094fc0-e8ad-4398-bbf3-14d0310a6e9d",
  "name": "My World",
  "description": "A place for things"
}
```

### RestUser
A subset of fields from Neos's `User` object.

| Field      | Type    | Description                              |
|------------|---------|------------------------------------------|
| id         | string  | the ID of the user                       |
| name       | string  | the name of the user                     |
| role       | string? | the role of the user                     |
| is_present | bool    | whether the user is present in the world |
| ping       | int     | the user's ping (in milliseconds)        |


#### Examples
```json
{
  "id": "u-31094fc0-e8ad-4398-bbf3-14d0310a6e9d",
  "name": "U-Someone",
  "role": null,
  "is_present": true,
  "ping": 10
}
```

### SessionAccessLevel
An enumeration of session access levels.

| Name             | Value | 
|------------------|-------|
| Private          | 0     |
| LAN              | 1     | 
| Friends          | 2     |
| FriendsOfFriends | 3     |
| RegisteredUsers  | 4     |
| Anyone           | 5     |

  > This type is defined by NeosVR in CloudX.Shared.


## Routes
### `GET` /worlds `200 OK`
Gets the worlds currently loaded by the client.

#### Parameters
None.

#### Returns
An array of [RestWorld](#RestWorld) objects. The array may be empty if no worlds
are loaded.

#### Errors
None.

#### Examples
```json
[
  {
    "id": "s-31094fc0-e8ad-4398-bbf3-14d0310a6e9d",
    "name": "My World",
    "description": "A place for things"
  },
  {
    "id": "s-a923ccdd-798f-4484-bb8f-bc9b43924cba",
    "name": "My other World",
    "description": "A place for other things"
  }
]
```

### `GET` /worlds/[{id}](#RestWorld) `200 OK`
Gets a specific world currently loaded by the client.

#### Parameters
None

#### Returns
A [RestWorld](#RestWorld) object.

#### Errors
* `404 Not Found` if no world with the given session ID is loaded by the
  client.

#### Examples
```json
{
  "id": "s-31094fc0-e8ad-4398-bbf3-14d0310a6e9d",
  "name": "My World",
  "description": "A place for things"
}
```

### `POST` /worlds `201 Created`
Starts a new world.

#### Parameters
| Field    | Type   | Description                               |
|----------|--------|-------------------------------------------|
| url      | string | the Neos record URI of the world to start |
| template | string | the name of a builtin world template      |

> `url` and `template` are mutually exclusive.

#### Returns
An asynchronous [Job](#Job).

#### Errors
* `404 Not Found` if no known template was found with the given name
* `400 Bad Request` if neither `url` nor `template` was provided

#### Examples
```json
{
  "id": "31094fc0-e8ad-4398-bbf3-14d0310a6e9d",
  "description": "start world SpaceWorld",
  "status": 0
}
```

### `POST` /worlds/[{id}](#RestWorld)/save `200 OK`
Saves the world identified by `id`.

#### Parameters
None.

#### Returns
An asynchronous [Job](#Job).

#### Errors
* `404 Not Found` if no world with the given ID was found
* `403 Forbidden` if the world cannot be saved 

#### Examples
```json
{
  "id": "192fb88e-4581-47a1-bc2f-b2be01e65b09",
  "description": "save world SpaceWorld",
  "status": 0
}
```

### `DELETE` /worlds/[{id}](#RestWorld) `204 No Content`
Closes the world identified by `id`.

#### Parameters
None.

#### Returns
Nothing.

#### Errors
* `404 Not Found` if no world with the given ID was found

### `POST` /worlds/[{id}](#RestWorld)/restart `200 OK`
Restarts the world identified by `id`.

#### Parameters
None.

#### Returns
An asynchronous [Job](#Job).

#### Errors
* `404 Not Found` if no world with the given ID was found OR it does not have
  a world handler

#### Examples
```json
{
  "id": "01a0605f-e0db-4789-9d17-6c76d57ab7ae",
  "description": "restart world SpaceWorld",
  "status": 0
}
```

### `PATCH` /worlds/[{id}](#RestWorld) `200 OK`
Modifies properties of the world identified by `id`.

#### Parameters

| Field               | Type                                       | Description                                       |
|---------------------|--------------------------------------------|---------------------------------------------------|
| ?name               | string                                     | the new name of the world                         |
| ?description        | string                                     | the new description of the world                  |
| ?access_level       | [SessionAccessLevel](#SessionAccessLevel)  | the new access level of the world.                |
| ?away_kick_interval | float                                      | the new kick interval for away users (in minutes) |

  > While all parameters to this route are optional, at least one must be set.

#### Returns
The updated [RestWorld](#RestWorld).

#### Errors
* `404 Not Found` if no world with the given ID was found
* `400 Bad Request` if 
  1. no parameters are provided OR
  2. `access_level` is not a recognized value OR
  3. `away_kick_interval` is not a number greater than or equal to zero

#### Examples
```json
{
  "id": "s-31094fc0-e8ad-4398-bbf3-14d0310a6e9d",
  "name": "My New World Name",
  "description": "A place for things"
}
```

### `GET` /worlds/[{id}](#RestWorld)/users `200 OK`
Gets the users currently in the world identified by `id`.

#### Parameters
None.

#### Returns
An array of [RestUser](#RestUser) objects.

#### Errors
* `404 Not Found` if no world with the given ID was found

#### Examples
```json
[
  {
    "id": "u-31094fc0-e8ad-4398-bbf3-14d0310a6e9d",
    "name": "U-Someone",
    "role": null,
    "is_present": true,
    "ping": 10
  }
]
```

### `GET` /worlds/[{id}](#RestWorld)/users/[{id}](#RestUser) `200 OK`
Gets a specific user currently in the world identified by `id`.

#### Parameters
None.

#### Returns
A [RestUser](#RestUser) object.

#### Errors
* `404 Not Found` if 
  1. no world with the given ID was found OR
  2. no user with the given ID was found

#### Examples
```json
{
  "id": "u-31094fc0-e8ad-4398-bbf3-14d0310a6e9d",
  "name": "U-Someone",
  "role": null,
  "is_present": true,
  "ping": 10
}
```

### `GET` /worlds/focused `200 OK`
Gets the currently focused world.

#### Parameters
None

#### Returns
A [RestWorld](#RestWorld) object.

#### Errors
* `404 Not Found` if no world is currently focused

#### Examples
```json
{
  "id": "s-31094fc0-e8ad-4398-bbf3-14d0310a6e9d",
  "name": "My World",
  "description": "A place for things"
}
```

### `PUT` /worlds/focused `200 OK`
Sets the currently focused world.

#### Parameters

| Field  | Type   | Description                    |
|--------|--------|--------------------------------|
| ?id    | string | the id of the world to focus   |
| ?name  | string | the name of the world to focus |
| ?index | int    | the loaded index of the world  |

  > At least one of `id`, `name`, or `index` must be set.

#### Returns
A [RestWorld](#RestWorld) object.

#### Errors
* `404 Not Found` if no world is currently focused

#### Examples
```json
{
  "id": "s-31094fc0-e8ad-4398-bbf3-14d0310a6e9d",
  "name": "My World",
  "description": "A place for things"
}
```