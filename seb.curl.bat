@echo off

REM --------------------------------------------------
REM Sports Exercise Battle (SEB)
REM --------------------------------------------------
title Sports Exercise Battle (SEB)
echo CURL Testing for Sports Exercise Battle (SEB)
echo.

REM --------------------------------------------------
echo 1) Create Users (Registration)
REM Create User
curl -i -X POST http://localhost:10001/users --header "Content-Type: application/json" -d "{\"Username\":\"kienboec\", \"Password\":\"daniel\"}"
echo "Should return HTTP 201"
echo.
curl -i -X POST http://localhost:10001/users --header "Content-Type: application/json" -d "{\"Username\":\"altenhof\", \"Password\":\"markus\"}"
echo "Should return HTTP 201"
echo.

echo should fail:
curl -i -X POST http://localhost:10001/users --header "Content-Type: application/json" -d "{\"Username\":\"kienboec\", \"Password\":\"daniel\"}"
echo "Should return HTTP 4xx User already exists"
echo.
curl -i -X POST http://localhost:10001/users --header "Content-Type: application/json" -d "{\"Username\":\"kienboec\", \"Password\":\"different\"}"
echo "Should return HTTP 4xx User already exists"
echo. 
echo.

REM --------------------------------------------------
echo 2) Login Users
curl -i -X POST http://localhost:10001/sessions --header "Content-Type: application/json" -d "{\"Username\":\"kienboec\", \"Password\":\"daniel\"}"
echo.
echo "Should return HTTP 200 Login successful + usertoken -> kienboec-sebtoken"
echo.
curl -i -X POST http://localhost:10001/sessions --header "Content-Type: application/json" -d "{\"Username\":\"altenhof\", \"Password\":\"markus\"}"
echo.
echo "Should return HTTP 200 Login successful + usertoken -> altenhof-sebtoken"
echo.

echo should fail:
curl -i -X POST http://localhost:10001/sessions --header "Content-Type: application/json" -d "{\"Username\":\"kienboec\", \"Password\":\"different\"}"
echo.
echo "Should return HTTP 401 Invalid username or password"
echo.
echo.


REM --------------------------------------------------
echo 3) edit user data
echo.
curl -i -X GET http://localhost:10001/users/kienboec --header "Authorization: Basic kienboec-sebToken"
echo.
curl -i -X GET http://localhost:10001/users/altenhof --header "Authorization: Basic altenhof-sebToken"
echo.
curl -i -X PUT http://localhost:10001/users/kienboec --header "Content-Type: application/json" --header "Authorization: Basic kienboec-sebToken" -d "{\"Name\": \"Kienboeck\",  \"Bio\": \"me playin...\", \"Image\": \":-)\"}"
echo.
echo "Should return HTTP 200 User updated"
echo.
curl -i -X PUT http://localhost:10001/users/altenhof --header "Content-Type: application/json" --header "Authorization: Basic altenhof-sebToken" -d "{\"Name\": \"Altenhofer\", \"Bio\": \"me codin...\",  \"Image\": \":-D\"}"
echo.
echo "Should return HTTP 200 User updated"
echo.
curl -i -X GET http://localhost:10001/users/kienboec --header "Authorization: Basic kienboec-sebToken"
echo.
curl -i -X GET http://localhost:10001/users/altenhof --header "Authorization: Basic altenhof-sebToken"
echo.
echo.
echo should fail:
curl -i -X GET http://localhost:10001/users/altenhof --header "Authorization: Basic kienboec-sebToken"
echo.
curl -i -X GET http://localhost:10001/users/kienboec --header "Authorization: Basic altenhof-sebToken"
echo.
curl -i -X PUT http://localhost:10001/users/kienboec --header "Content-Type: application/json" --header "Authorization: Basic altenhof-sebToken" -d "{\"Name\": \"Hoax\",  \"Bio\": \"me playin...\", \"Image\": \":-)\"}"
echo.
curl -i -X PUT http://localhost:10001/users/altenhof --header "Content-Type: application/json" --header "Authorization: Basic kienboec-sebToken" -d "{\"Name\": \"Hoax\", \"Bio\": \"me codin...\",  \"Image\": \":-D\"}"
echo.
curl -i -X GET http://localhost:10001/users/someGuy  --header "Authorization: Basic kienboec-sebToken"
echo.
echo.

REM --------------------------------------------------
echo 4) stats (get my elo value and count of pushups overall; startup value e.g. 100)
curl -i -X GET http://localhost:10001/stats --header "Authorization: Basic kienboec-sebToken"
echo.
curl -i -X GET http://localhost:10001/stats --header "Authorization: Basic altenhof-sebToken"
echo.
echo.

REM --------------------------------------------------
echo 5) scoreboard (compare elo values and count of pushups accross all users)
curl -i -X GET http://localhost:10001/score --header "Authorization: Basic kienboec-sebToken"
echo.
echo.

REM --------------------------------------------------
echo 6) history (count and duration; currently empty)
curl -i -X GET http://localhost:10001/history --header "Authorization: Basic kienboec-sebToken"
echo.
curl -i -X GET http://localhost:10001/history --header "Authorization: Basic altenhof-sebToken"
echo.
echo.

REM --------------------------------------------------
echo 7) list current tournament info/state (currently none)
curl -i -X GET http://localhost:10001/tournament --header "Authorization: Basic kienboec-sebToken"
echo.
curl -i -X GET http://localhost:10001/tournament --header "Authorization: Basic altenhof-sebToken"
echo.
echo.

REM --------------------------------------------------
echo 8) add entry to history / starts a tournament
curl -i -X POST http://localhost:10001/history --header "Content-Type: application/json" --header "Authorization: Basic kienboec-sebToken" -d "{\"Name\": \"PushUps\",  \"Count\": 40, \"DurationInSeconds\": 60}"
echo.
curl -i -X POST http://localhost:10001/history --header "Content-Type: application/json" --header "Authorization: Basic altenhof-sebToken" -d "{\"Name\": \"PushUps\",  \"Count\": 50, \"DurationInSeconds\": 70}"
echo.
REM echo should fail:
REM curl -i -X POST http://localhost:10001/history --header "Content-Type: application/json" --header "Authorization: Basic altenhof-sebToken" -d "{\"Name\": \"PushUps\",  \"Count\": 50, \"DurationInSeconds\": 70}"
echo.
echo.

REM --------------------------------------------------
echo 9) list current tournament info/state (tournament started; 2 participants; altenhof in front; write start-time)
curl -i -X GET http://localhost:10001/tournament --header "Authorization: Basic kienboec-sebToken"
echo.
curl -i -X GET http://localhost:10001/tournament --header "Authorization: Basic altenhof-sebToken"
echo.
echo.

REM --------------------------------------------------
echo 10) stats (get my elo value and count of pushups overall; startup value like 100 - no tournament should be finished here)
curl -i -X GET http://localhost:10001/stats --header "Authorization: Basic kienboec-sebToken"
echo.
curl -i -X GET http://localhost:10001/stats --header "Authorization: Basic altenhof-sebToken"
echo.
echo.

REM --------------------------------------------------
echo 11) scoreboard (compare elo values and count of pushups accross all users; still startup values)
curl -i -X GET http://localhost:10001/score --header "Authorization: Basic kienboec-sebToken"
echo.
echo.

REM --------------------------------------------------
echo 12) history (count and duration; 1 entry each)
curl -i -X GET http://localhost:10001/history --header "Authorization: Basic kienboec-sebToken"
echo.
curl -i -X GET http://localhost:10001/history --header "Authorization: Basic altenhof-sebToken"
echo.
echo.

REM --------------------------------------------------
echo 13) add entry to history / continues in tournament
curl -i -X POST http://localhost:10001/history --header "Content-Type: application/json" --header "Authorization: Basic kienboec-sebToken" -d "{\"Name\": \"PushUps\",  \"Count\": 11, \"DurationInSeconds\": 25}"
echo.
echo.

REM --------------------------------------------------
echo 14) list current tournament info/state 
curl -i -X GET http://localhost:10001/tournament --header "Authorization: Basic kienboec-sebToken"
echo.
curl -i -X GET http://localhost:10001/tournament --header "Authorization: Basic altenhof-sebToken"
echo.
echo.

REM --------------------------------------------------
echo 15) sleep of 2min (afterwards the tournament should be over and elo values need to be updated)
ping localhost -n 120 >NUL 2>NUL
echo.
echo.

REM --------------------------------------------------
echo 16) list current tournament info/state (1 tournament with state ended)
curl -i -X GET http://localhost:10001/tournament --header "Authorization: Basic kienboec-sebToken"
echo.
curl -i -X GET http://localhost:10001/tournament --header "Authorization: Basic altenhof-sebToken"
echo.
echo.

REM --------------------------------------------------
echo 17) stats
curl -i -X GET http://localhost:10001/stats --header "Authorization: Basic kienboec-sebToken"
echo.
curl -i -X GET http://localhost:10001/stats --header "Authorization: Basic altenhof-sebToken"
echo.
echo.

REM --------------------------------------------------
echo 18) scoreboard 
curl -i -X GET http://localhost:10001/score --header "Authorization: Basic kienboec-sebToken"
echo.
echo.

REM --------------------------------------------------
echo 19) history 
curl -i -X GET http://localhost:10001/history --header "Authorization: Basic kienboec-sebToken"
echo.
curl -i -X GET http://localhost:10001/history --header "Authorization: Basic altenhof-sebToken"
echo.
echo.

REM --------------------------------------------------
echo end...


@echo on

pause