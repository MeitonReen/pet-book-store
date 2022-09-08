import React, { useState, useEffect } from "react";
import { useSearchParams, useNavigate } from "react-router-dom";
import "./default-layout.css"
import "../../index.css"

function DefaultLayout(props) {
    return (
        <div className='default-layout'>
            <div className='default-layout-body'>
                <p className='default-layout__title'>BlackEdgeBookStore<br />Authorization server</p>
                <div className='default-layout__content'>
                    {props.children}
                </div>
            </div>
        </div>
    );
}
export default DefaultLayout;