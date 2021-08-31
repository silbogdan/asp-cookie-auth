import { useEffect, useState } from 'react';
import './App.css';
import Login from './Components/Login';
import axios from 'axios';

axios.defaults.withCredentials = true;

function App() {

  const [isLogged, setLogged] = useState();

  useEffect(() => {
    const checkLogged = async () => {
      try {
        const response = await axios.get('https://localhost:44333/api/auth/isLogged');
        setLogged(() => 1);
        console.log(response.status);
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
