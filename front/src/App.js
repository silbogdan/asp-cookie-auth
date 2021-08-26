import { useEffect, useState } from 'react';
import './App.css';
import Login from './Components/Login';
import axios from 'axios';

axios.defaults.withCredentials = true;

function App() {

  const [isLogged, setLogged] = useState();

  useEffect(() => {
    const checkLogged = async () => {
      const response = await axios.get('https://localhost:44333/api/auth/isLogged');

      console.log(response.status);
      if (response.status === 401)
        setLogged(() => false);
      else
        setLogged(() => true);
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
