import React, { useState, useEffect } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import StatusCodeService from '../../../custom-libs/StatusCodeService';
import './sign-in-page.css'
import '../../../index.css'

function SignInPage(props) {
    const urls = props.urls;

    const [searchParams] = useSearchParams();

    const [login, setLogin] = useState('');
    const [password, setPassword] = useState('');

    const [errorMessage, setErrorMessage] = useState('');

    const navigate = useNavigate();

    return (
        <div className='sign-in-page' onKeyDown={e => downEnter(e)}>
            <label className='sign-in-page__login-title' htmlFor='sign-in-page__login'>Login:</label>
            <input className='sign-in-page__login-input' autoFocus type='text' id='sign-in-page__login'
                value={login} onChange={e => updateLogin(e.target.value)} />

            <label className='sign-in-page__password-title' htmlFor='sign-in-page__password'>Password:</label>
            <input className='sign-in-page__password-input' type='password' id='sign-in-page__password'
                value={password} onChange={e => updatePassword(e.target.value)} />

            <button onClick={() => tryAuthorize()} className='sign-in-page__sign-in-button'>SignIn</button>

            <p className='sign-in-page__error-message'>{errorMessage}</p>
        </div>
    );
    async function tryAuthorize() {
        const response = await fetch(
            `${urls.oPAuthorizeEndpoint}?${searchParams.toString()}`
            + '&prompt=consent'
            + `&username=${login}`
            + `&password=${password}`);

        new StatusCodeService()
            .if([401], response, () => setErrorMessage('Invalid login or password'))
            .if([200], response, () => {
                setErrorMessage('');

                navigate(
                    `${urls.consent}?${searchParams.toString()}`
                    + `&username=${login}`
                    + `&password=${password}`);
            })

        return;
    }
    function updateLogin(newValue) {
        setErrorMessage('');
        setLogin(newValue);
    }
    function updatePassword(newValue) {
        setErrorMessage('');
        setPassword(newValue);
    }
    function downEnter(event){
        if (event.keyCode === 13) {
            tryAuthorize();
        }
    }
}
export default SignInPage;