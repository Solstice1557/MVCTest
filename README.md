Part 1:

Please develop a webform or MVC web app that has 3 users: MasterUser, User1 and User2 (store those info on a config file) and three pages: 1- login page 2- welcome page (For authenticated user only) 3- MasterUser's Page (for authenticated MasterUser only)

1- User MUST login to the web app. Then the app creates a session for each successfull authentication then redirects the user to the welcome page

2- MasterUser should be able to enforce  'logout' to other users remotely (User1 or User2) by clicking a button. By enforcing, that user's session should get expired and the user should 'immediately' get redirected to login page.

Feel free to use any third party libraries for Part 1.

Part 2:

Come up with an idea that prevents DoS/Brute-force attack and implement it. So a hacker can't use brute-force methods to guess the password or DoS to bring down the server. We don't expect full development here. Just some general implementation.You cannot use third party libraries for Robot detection like Google reCaptcha