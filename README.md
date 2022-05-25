# MaKore
## Best Way to Communicate

### About
MaKore is a communication app we build during the semester<br />
This is the second part - Web Server.<br />
In addition to the pervious part, we used and experienced with: ASP.net and C#, signalR, MVC design pattern, MySQL database, JSON Web Token (JWT) and services.<br />

### About the Server
Our website's server has to be connected in order to be able to login or register to MaKore. <br />
The server holds a DB with all of MaKore's users and chats information and messages. <br />
It supports HTTP's GET, POST, PUT and DELETE requesets and returns or updates the relevent inforamtion. <br />
Besides responding to our clients, the server can also recieve requests from other servers. Thus, MaKore's users can chat with other users from different websites that also implement the same API as our server. <br />

This is our very first encounter with MVC - a rating page for our app.<br />
You can add a review, edit and delete an existing review, and also go back to the chat if you are a connected user.<br />
<br />
![rating](https://user-images.githubusercontent.com/90967892/170185970-1db8c50a-02dc-4688-8955-4035c0910284.jpg)<br />
<br />
Notice that there is a "Return to Chat" button. Going back to the chat will only be possible when a user has logged in and pressed the rating button in the chat (see an explenation below). Since we use JWT, when this user wants to go back to the chat he will be redirected to the login page if the time session has passed. <br />
<br />
Add a new rating:<br />
<br />
![newRating](https://user-images.githubusercontent.com/90967892/170186391-740d62fe-932f-4126-9329-5aaa7bf83de5.jpg)<br />
<br />
See details of an existing rating and edit it:<br />
<br />
![detailsRating](https://user-images.githubusercontent.com/90967892/170186403-5401c431-c7a3-4fc1-a7dd-810b706f19cd.jpg)<br />
<br />


### About the MaKore Website
Since we already designed the chat in the first assigment, here we only present the differecnes. <br />

#### Sign Up page:
Now, our users are no longer hard coded. A new user can register and his details will be passed to the server. <br />
<br />
![signup](https://user-images.githubusercontent.com/90967892/164452688-3818a90f-e764-47b8-99f7-fed8fe7ff71c.jpg)<br />
<br />
All the requirments stay the same.

#### Sign In page:
An existing user that has registered exists in the DB of the user. Thus he can log in.<br />
Of course the server has to be connected for this. Thus, if the server is offline or if something happed we let the user know. <br />
<br />
![serverError](https://user-images.githubusercontent.com/90967892/170189297-47eb1116-86d3-479c-aea2-ce5e2307944d.jpg)<br />
<br />
Besides that, the autantication we do in order to validate the user stays the same, with the exception that now it's done by the sevrer.

#### Chat page:
Here is the big improvement.<br />

First, we added a button connecting the chat to the **rating page**: <br />
<br />
![ratingSym](https://user-images.githubusercontent.com/90967892/170192840-7acf343a-757b-45cd-83f0-5b795979fdf3.jpg)
<br />

Now let's take a look at corali which has no conversations yet. <br />
In order to **add a new chat**, the user has to know the contact's server. This way the user's client-side ?????? will know who to transfer the invitation to. <br />
<br />
![addContact](https://user-images.githubusercontent.com/90967892/170191209-5e036d7e-2529-4fdb-a9ae-74f5917235ef.jpg)<br />
<br />
![specificContatc](https://user-images.githubusercontent.com/90967892/170191745-657a90e1-00cd-4bbb-9441-19e633582783.jpg) <br />
<br />
When trying to add another server's user, we let the user know it may take awhile due to the communication with another server. <br />
 <br />
![diffServer1](https://user-images.githubusercontent.com/90967892/170193752-68d1d666-63b5-4aad-8990-ae9faa3d7147.jpg)<br />
<br />
When an error has occured (for example if the other server is not availabe) we also tell the user. <br />
 <br />
![diffServer2](https://user-images.githubusercontent.com/90967892/170193909-701c7510-eccc-4ec2-a7e8-0bde1287b0cd.jpg)<br />
<br />
And if we could reach the other server, we tell the user the invitation was succesfully sent. <br />
<br />
![invitation](https://user-images.githubusercontent.com/90967892/170194735-c13f4192-a427-4536-ad72-c0a8d731214e.jpg)<br />

corali and tal are both users of the MaKore's server. Since we support **online communication** (using signalR), the message corali wrote to tal appeared at tal's website right away in a new chat between them:<br />
<br />
![realTimeMess](https://user-images.githubusercontent.com/90967892/170192114-3ea920d0-f990-42f2-a57b-d43186e7ac16.jpg) <br />
<br />


#### Notes:
- We only support text messages now.
- Searching users in the chat is still available.
- There is one picture (of MaKore) for all users.

### How to Run
Our project divides into two servers - two repositories.
1. Clone the repository:
    ```
    git clone https://github.com/CoralKuta/MaKore/Chat
    ```
2. Enter the MaKore folder:
    ```
    cd MaKore
    ```
3. Install all modules:
    ```
    npm install
    ```
4. Run the program:
    ```
    npm start
    ```
5. Enter the following details:
    ```
    Username: Ido
    Password: 12341234a
    ``` 
6. Explore !


**A note:** There are several users but Ido is the main user for now. It is possible to log in with any user you want. for example:
<br />
Username: Ido<br />
Password: 12341234a
<br />
<br />
Username: Coral<br />
Password: 12345678ab
<br />
<br />
Username: Tal<br />
Password: 1234567890a
<br />
<br />
Username: Ariel<br />
Password: 1234aaaa
<br />
for more useres you can check **users.js** at src folder.<br />
<br />
Also, Bella is not Ido's friend. When logged in to Ido's account, you can add her as a new friend. 

### Demo
![ourGif](https://user-images.githubusercontent.com/92373590/164725983-68a33597-394a-4502-9c54-5a60acf0fcfe.gif)

### Developers
- Tal Dabran 316040898
- Ido Tavron 316222512
- Coral Kuta 208649186

