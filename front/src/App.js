import { useEffect, useState } from 'react';
import './App.css';
import Login from './Components/Login';
import axios from 'axios';

axios.defaults.withCredentials = true;
axios.interceptors.response.use((response) => {
  return response
}, (err) => {
  if (err.response.status === 401) {
    console.log('Am primit 401');
    Promise.reject(err);
  }
});

function App() {

  const [isLogged, setLogged] = useState();

  useEffect(() => {
    const checkLogged = async () => {
      try {
        const response = await axios.get('https://localhost:44333/api/auth/isLogged');
        console.log(response.data);
        if (response.data === 1) {
          setLogged(() => 1);
        } else {
          setLogged(() => 0);
        }
      } catch (err) {
        setLogged(() => 0);
      }
      
    }

    checkLogged();
  }, []);

  const logout = async () => {
    await axios.get('https://localhost:44333/api/auth/logout');
    setLogged(() => false);
  }

  if (!isLogged) {
    return (
      <>
        <Login setLogged={setLogged} />
      </>
    );
  } else {
    return (
      <>
        <button onClick={async () => await logout()}>Logout</button>
      </>
    )
  }
}

export default App;
