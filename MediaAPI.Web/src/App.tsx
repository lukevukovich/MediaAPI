import {
  createBrowserRouter,
  createRoutesFromElements,
  Route,
  RouterProvider,
} from "react-router-dom";

// Layouts and pages
import RootLayout from "./utils/RootLayout";
import Home from "./pages/Home/Home";
import ApiDocs from "./pages/ApiDocs/ApiDocs";
import Stremio from "./pages/Stremio/Stremio";

import "./App.css";

// Router and routes
const router = createBrowserRouter(
  createRoutesFromElements(
    <Route path="/" element={<RootLayout />}>
      <Route index element={<Home />} />
      <Route path="docs" element={<ApiDocs />} />
      <Route path="stremio" element={<Stremio />} />
    </Route>
  )
);

function App() {
  return <RouterProvider router={router} />;
}

export default App;
