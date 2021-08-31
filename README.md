# ASP Cookie Auth Template
This is a template for apps with ASP.NET backend that are built using Cookie Authentication.
Its purpose is to learn about this authentication method and then use it as a good starter for more complex apps.

## How does it work?
Services are configured to use Cookie Authentication method to create a user login session. It creates a cookie called 'UserLoginCookie' that contains Claims with the following structure: `ID: UserId, Username: username`.
On client-side the user is provided with a login form if not authenticated. When the user successfully logs in, a component with a logout button is rendered.

## Set-up
1) Create a SQL Server database with `Users` table and the following fields: 

    >   UserId (PK, int, not null) <br>
    >   Username (varchar(63), not null) <br>
    >   Password (varchar(255), not null)
  
2) Start the backend server using IIS Express
3) Start the frontend React server using `npm start`

## Dependencies
- ASP.NET 3.1
- React v17.0.2
- Node v14.17.5
