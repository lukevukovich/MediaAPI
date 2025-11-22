import Header from "../Header/Header";
import "./Home.css";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faLinkedin, faGithub } from "@fortawesome/free-brands-svg-icons";

export default function Home() {
  return (
    <div className="home">
      <Header />
      <h2>
        media-api is a platform designed to aggregate and deliver media data
        from multiple external sources through a unified RESTful API. Built with
        a .NET 8 backend and a React frontend, it streamlines access to
        comprehensive media information for developers and applications.
      </h2>
      <h2>
        The standard API provides endpoints for accessing MdbList media lists
        and retrieving metadata for individual media items. See the API
        documentation for all available endpoints, example requests, interactive
        testing, and integration guides.
      </h2>
      <h2>
        The Stremio addon lets you discover, organize, and stream media from
        multiple sources directly within Stremio. It provides several custom
        catalogs with advanced filter support, making it easy to find exactly
        what you want. See the addon documentation for installation instructions
        and usage details.
      </h2>
      <h2 className="note">
        media-api by Luke Vukovich
        <a
          href="https://www.linkedin.com/in/lukevuke/"
          target="_blank"
          rel="noopener noreferrer"
        >
          <FontAwesomeIcon icon={faLinkedin} className="icon" />
        </a>
        <a
          href="https://github.com/lukevukovich"
          target="_blank"
          rel="noopener noreferrer"
        >
          <FontAwesomeIcon icon={faGithub} className="icon" />
        </a>
      </h2>
    </div>
  );
}
