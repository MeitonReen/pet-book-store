import logo from './logo.svg';
import './App.css';
import { BrowserRouter, Routes, Route } from "react-router-dom";
import ConsentPage from './components/pages/consent/consent-page'
import SignUpPage from './components/pages/sign-up/sign-up-page'
import SignInPage from './components/pages/sign-in/sign-in-page'
import DefaultLayout from './components/default-layout/default-layout'
import AccessRequestedFrom from './components/access-requested-from/access-requested-from'

function App() {

  return (
    <BrowserRouter>
      <Routes>
        <Route path={urls.consent} element={
          <DefaultLayout>
            <AccessRequestedFrom>
              <ConsentPage oPAuthorizeEndpoint={urls.oPAuthorizeEndpoint} />
            </AccessRequestedFrom>
          </DefaultLayout>
        }>
        </Route>

        <Route path={urls.signUp} element={
          <DefaultLayout>
            <SignUpPage urls={urls} />
          </DefaultLayout>}>
        </Route>

        <Route path={urls.signIn} element={
          <DefaultLayout>
            <AccessRequestedFrom>
              <SignInPage urls={urls} />
            </AccessRequestedFrom>
          </DefaultLayout>}>
        </Route>

        {/* <Route path={urls.signOut} element={<SignOutPage />}>
        </Route> */}
      </Routes>
    </BrowserRouter>
  );
}
const urls = {
  consent: process.env.REACT_APP_CONSENT_PAGE_LOCAL_URI,
  signUp: process.env.REACT_APP_SIGN_UP_PAGE_LOCAL_URI,
  signIn: process.env.REACT_APP_SIGN_IN_PAGE_LOCAL_URI,
  oPAuthorizeEndpoint: process.env.REACT_APP_OP_AUTHORIZE_ENDPOINT_URI,
  oPAccountEndpoint: process.env.REACT_APP_OP_ACCOUNT_ENDPOINT_URI
}
export default App;
