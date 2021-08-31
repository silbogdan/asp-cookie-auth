import axios from 'axios';
import React, { useState } from 'react';

axios.defaults.withCredentials = true;

const Login = (props) => {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');

    const clickEvent = async (event) => {
        event.preventDefault();
    
        var config = {
            headers: { 
              'Authorization': 'Basic ' + btoa(username + ':' + password)
            }
          };
          
        const response = await axios.get('https://localhost:44333/api/auth/login', config);

        setPassword(() => '');
        setUsername(() => '');
        if (response.status === 200) {
            props.setLogged(() => true);
        }
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