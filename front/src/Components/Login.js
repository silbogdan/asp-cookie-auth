import axios from 'axios';
import React, { useState } from 'react';

axios.defaults.withCredentials = true;

const Login = () => {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');

    const clickEvent = async (event) => {
        event.preventDefault();
    
        var config = {
            method: 'get',
            url: 'https://localhost:44333/api/auth/login',
            headers: { 
              'Authorization': 'Basic ' + btoa(username + ':' + password)
            }
          };
          
          axios(config)
          .then(function (response) {
            console.log(JSON.stringify(response.data));
          })
          .catch(function (error) {
            console.log(error);
          });

        setPassword(() => '');
        setUsername(() => '');
    }

    return (
        <>
            <form>
                <input id="username-input" name="username-input" type="text" value={username} onChange={(event) => setUsername(event.target.value)} placeholder="Username" /> <br />
                <input id="pass-input" name="pass-input" type="password" value={password} onChange={(event) => setPassword(event.target.value)} placeholder="Password" /> <br />
                <button onClick={async (event) => await clickEvent(event)}>Login</button>
            </form>
        </>
    );
}

export default Login;