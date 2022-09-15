import React, { useState, useEffect } from 'react';
import { useSearchParams, useNavigate } from 'react-router-dom';
import './consent-page.css'
import '../../../index.css'

function ConsentPage(props) {
    const oPAuthorizeEndpoint = props.oPAuthorizeEndpoint;

    const [searchParams] = useSearchParams();
    const [scopesState, setScopesState] = useState(ScopesFromSearchParamsToObject(searchParams));

    const scopesAsJsx = Object
        .keys(scopesState)
        .map((scopeName, i) =>
            <div className='scope'>
                <input className='custom-checkbox' type='checkbox' id={'scope-checkbox' + i}
                    onChange={(event) => UpdateScopeState(scopeName, event.target.checked)}
                    checked={scopesState[scopeName]} />

                <label className='scope__scope-name' htmlFor={'scope-checkbox' + i}>{scopeName}</label>
            </div>
        );

    return (
        <div className='consent-page' onKeyDown={e => downEnter(e)} tabIndex={-1}>
            <p className='consent-page__title'>Requested permissions:</p>

            <div className='consent-page__scopes-management'>
                {scopesAsJsx}
                <hr className='consent_page__horizontal-rule' />

                <a onClick={() => SelectAllScopes()}>select all</a>

                <a onClick={() => SetNoneSelectedScopes()}>select none</a>
            </div>

            <button onClick={() => tryAuthorize()} className='button consent-page__allow-button'>Allow</button>
        </div>
    );
    async function tryAuthorize() {

        let scopeCheckedAsParam = '';

        Object.entries(scopesState).map(objEntry => {
            const [key, value] = objEntry;
            if (value) scopeCheckedAsParam += (key + ' ');
        });

        let authorizeRequestAfterConsentUrl = oPAuthorizeEndpoint
            + '?'
            + `client_id=${searchParams.get('client_id')}`
            + `&response_type=${searchParams.get('response_type')}`
            + `&username=${searchParams.get('username')}`
            + `&password=${searchParams.get('password')}`
            + `&redirect_uri=${searchParams.get('redirect_uri')}`
            + `&scope=${scopeCheckedAsParam}`
            + `&state=${encodeURIComponent(searchParams.get('state'))}`;

        let authorizeResponse = await fetch(authorizeRequestAfterConsentUrl, {
            mode: 'no-cors'
        });

        //Обход проблемы [Origin:null] с ajax redirect
        //Причём при [mode: 'no-cors', redirect: 'manual'] запрос вообще не отправится,
        //оверхед обхода: 4 vs 2 запросов, лучше пока не придумал.

        if (authorizeResponse.type == 'opaque' || authorizeResponse.ok) {
            window.location.replace(authorizeRequestAfterConsentUrl);
        }
        // let locationHeader = authorizeResponse.headers.get('Location');

        // let headers = authorizeResponse.headers;
        // let headerValues = authorizeResponse.headers.values();

        // console.log('headers = ', headers);
        // console.log('headerValues = ', headerValues);
        // console.log(authorizeResponse.url);

        // if (authorizeResponse.status != 200) return;
        // debugger;


        // let codeAndStateParamsFromLocation = new URL(authorizeResponse.url).searchParams.toString();
        // let searchParams = new URL(location).searchParams;
        // console.log('searchParams=',searchParams.toString());
        // navigate(searchParams.get('redirect_uri').slice(0, -1)
        //     + '?'
        //     + 'code=' + codeParam
        //     + 'client_id=' + searchParams.get('client_id')
        //     + 'redirect_uri=' + searchParams.get('redirect_uri'));
        // debugger;

        // window.location.replace(searchParams.get('redirect_uri')
        //     + '?'
        //     + codeAndStateParamsFromLocation);
        return;

    }
    function SelectAllScopes() {
        setScopesState(prevState =>
            Object.keys(prevState).reduce((newState, prevStateScopeName) => {
                newState[prevStateScopeName] = true;
                return newState;
            }, {})
        );
    }
    function SetNoneSelectedScopes() {
        setScopesState(prevState =>
            Object.keys(prevState).reduce((newState, prevStateScopeName) => {
                newState[prevStateScopeName] = false;
                return newState;
            }, {})
        );
    }
    function UpdateScopeState(scopeName, scopeState) {
        setScopesState(prevValue => {
            return { ...prevValue, [scopeName]: scopeState }
        })
    }
    function ScopesFromSearchParamsToObject(searchParams) {
        return searchParams
            .get('scope')
            .split(' ')
            .reduce((acc, scopeName) => {
                acc[scopeName] = false;
                return acc;
            }, {});
    }
    function downEnter(event){
        if (event.keyCode === 13) {
            tryAuthorize();
        }
    }
}
export default ConsentPage;