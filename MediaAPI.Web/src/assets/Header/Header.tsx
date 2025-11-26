import "./Header.css";
import "../../App.css";
import { useNavigate } from "react-router-dom";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faCode } from "@fortawesome/free-solid-svg-icons";

export default function Header() {
  const navigate = useNavigate();

  return (
    <div className="header">
      <h1 onClick={() => navigate("/")}>media-api</h1>
      <h1 className="small-home-button" onClick={() => navigate("/")}><FontAwesomeIcon icon={faCode} /></h1>
      <button onClick={() => navigate("/docs")}>API Docs</button>
      <button onClick={() => navigate("/stremio")}>Stremio</button>
    </div>
  );
}
