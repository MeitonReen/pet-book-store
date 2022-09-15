import React, { useState, useEffect } from 'react';
import { Redirect, useLocation } from "react-router-dom";
import "./sign-out-page.css"
import "../../../index.css"

function SignOutPage() {
    return (
        <div className="page sign-out-page">
            <div className="sign-out-page__body">

                <p className="sign-out-page__title">Confirm?</p>

                <button className="button sign-out-page__sign-out-button">SignOut</button>
            </div>
        </div>
    );
    //localhost:3001/account/sign-out
}
export default SignOutPage;