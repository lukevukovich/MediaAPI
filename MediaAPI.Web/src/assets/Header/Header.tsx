import "./Header.css";
import "../../App.css"
import { useNavigate } from "react-router-dom";

export default function Header() {
  const navigate = useNavigate();

  return (
    <div className="header">
      <h1 onClick={() => navigate("/")}>media-api</h1>
      <button onClick={() => navigate("/docs")}>Documentation</button>
      <button onClick={() => navigate("/stremio")}>Stremio</button>
    </div>
  );
}