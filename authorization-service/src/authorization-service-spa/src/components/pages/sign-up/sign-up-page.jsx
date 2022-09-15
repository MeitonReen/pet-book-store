import React, { useState, useEffect } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import './sign-up-page.css'
import '../../../index.css'

function SignUpPage(props) {
    const urls = props.urls;

    const [searchParams] = useSearchParams();

    const [login, setLogin] = useState('');
    const [password, setPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');

    const [accountIsCreated, setAccountIsCreated] = useState(false);

    const navigate = useNavigate();

    return (
        !accountIsCreated ?
            <div className='sign-up-page' onKeyDown={e => downEnter(e)}>
                <label className='sign-up-page__login-title' htmlFor='sign-up-page__login'>Login:</label>
                <input className='sign-up-page__login-input' type='text' id='sign-up-page__login'
                    value={login} onChange={e => setLogin(e.target.value)} />

                <label className='sign-up-page__password-title' htmlFor='sign-up-page__password'>Password:</label>
                <input className='sign-up-page__password-input' type='password' id='sign-up-page__password'
                    value={password} onChange={e => setPassword(e.target.value)} />

                <label className='sign-up-page__confirm-password-title'
                    htmlFor='sign-up-page__confirm-password'>Confirm password:</label>
                <input className='sign-up-page__confirm-password-input' type='password'
                    id='sign-up-page__confirm-password'
                    value={confirmPassword} onChange={e => setConfirmPassword(e.target.value)} />

                <button onClick={() => tryCreateAccount()} className='sign-up-page__sign-up-button'>SignUp</button>
            </div> :
            <div>{navigate(urls.signIn + '?' + searchParams.toString())}</div>
    );
    async function tryCreateAccount() {
        const response = await fetch(urls.oPAccountEndpoint, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            },
            body: `login=${login}&password=${password}`,
            cache: 'no-cache'
        });

        if (response.status == 200) {
            setAccountIsCreated(true);
        }

        setAccountIsCreated(false);
        return;
    }
    function downEnter(event){
        if (event.keyCode === 13) {
            tryCreateAccount();
        }
    }
}
export default SignUpPage;