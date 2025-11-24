import {
  createBrowserRouter,
  createRoutesFromElements,
  Route,
  RouterProvider,
} from "react-router-dom";

// Layouts and pages
import RootLayout from "./utils/RootLayout";
import Home from "./pages/Home/Home";
import Documentation from "./pages/Documentation/Documentation";
import Stremio from "./pages/Stremio/Stremio";

import "./App.css";

// Router and routes
const router = createBrowserRouter(
  createRoutesFromElements(
    <Route path="/" element={<RootLayout />}>
      <Route index element={<Home />} />
      <Route path="docs" element={<Documentation />} />
      <Route path="stremio" element={<Stremio />} />
    </Route>
  )
);

function App() {
  return <RouterProvider router={router} />;
}

export default App;
