import Header from "../../assets/Header/Header";
import "./Stremio.css";
import ExpandPanel from "../../assets/ExpandPanel/ExpandPanel";
// import { useEffect, useState } from "react";
// import { getApiMetadata, type ApiMetadata } from "../../utils/ApiMetadata";
import Settings from "../../Settings";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faCircleInfo,
  faCloudDownload,
  faFilm,
  faPuzzlePiece,
} from "@fortawesome/free-solid-svg-icons";

export default function Stremio() {
  const settings = new Settings();

  // const [apiMetadata, setApiMetadata] = useState<ApiMetadata | null>(null);

  // useEffect(() => {
  //   getApiMetadata().then(setApiMetadata);
  // }, []);

  return (
    <div className="stremio-page">
      <Header />
      <div className="stremio-section">
        <h2 className="tag">
          Addon
          <FontAwesomeIcon
            className="tag-icon"
            icon={faPuzzlePiece}
          ></FontAwesomeIcon>
        </h2>
        <ExpandPanel
          titleChildren={
            <p className="stremio-panel-title">
              About
              <FontAwesomeIcon
                className="panel-icon"
                icon={faCircleInfo}
              ></FontAwesomeIcon>
            </p>
          }
          expandChildren={
            <div className="stremio-panel-content">
              <p>
                The media-api Stremio addon provides access to rich media
                catalogs and metadata directly within the Stremio app. It
                aggregates data from multiple external sources to deliver a
                seamless media discovery and streaming experience.
              </p>
              <p>
                By integrating the media-api addon, Stremio users can explore a
                vast library of movies, TV shows, and other content, complete
                with detailed information, cover images, and streaming links.
              </p>
            </div>
          }
        ></ExpandPanel>
        <ExpandPanel
          titleChildren={
            <p className="stremio-panel-title">
              Installation
              <FontAwesomeIcon
                icon={faCloudDownload}
                className="panel-icon"
              ></FontAwesomeIcon>
            </p>
          }
          expandChildren={
            <div className="stremio-panel-content">
              <p>
                To install the media-api addon, simply navigate to the Stremio
                Addons section and select "Add addon". Enter the following
                manifest URL, click Add, and Install. The addon will then be
                available for use within Stremio.
              </p>
              <div className="stremio-manifest-url-container">
                <p className="stremio-manifest-url-label">Manifest URL:</p>
                <p className="stremio-manifest-url">
                  {settings.mediaApiUrl}stremio/manifest.json
                </p>
              </div>
            </div>
          }
        ></ExpandPanel>
      </div>
      <div className="stremio-section">
        <h2 className="stremio-tag">
          Catalogs
          <FontAwesomeIcon className="tag-icon" icon={faFilm}></FontAwesomeIcon>
        </h2>
      </div>
    </div>
  );
}
