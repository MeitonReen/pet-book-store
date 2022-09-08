import React, { useState, useEffect } from "react";
import { useSearchParams, useNavigate } from "react-router-dom";
import "./access-requested-from.css"
import "../../index.css"

function AccessRequestedFrom(props) {
    const [searchParams] = useSearchParams();

    const clientId = searchParams.get("client_id") ?? 'not found';

    return (
        <div className='access-requested-from'>
            <div className='access-requested-from-body'>
                <p className='access-requested-from__access-requested-from-title'>Access requested from:<br />
                    <span className='access-requested-from__access-requested-from-value'>{clientId}</span></p>
                <div className='access-requested-from__content'>
                    {props.children}
                </div>
            </div>
        </div>
    );
}
export default AccessRequestedFrom;