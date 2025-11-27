import Header from "../../assets/Header/Header";
import "./Stremio.css";
import ExpandPanel from "../../assets/ExpandPanel/ExpandPanel";
import { useEffect, useState } from "react";
import {
  getApiMetadata,
  type ApiMetadata,
  type CatalogMetadata,
} from "../../utils/ApiMetadata";
import Settings from "../../Settings";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faCheck,
  faCircleExclamation,
  faCircleInfo,
  faCloudDownload,
  faFilm,
  faPuzzlePiece,
  faSpinner,
  faTimes,
  type IconDefinition,
} from "@fortawesome/free-solid-svg-icons";
import { faCopy } from "@fortawesome/free-regular-svg-icons";

export default function Stremio() {
  const settings = new Settings();

  const [endpoints, setEndpoints] = useState<string[]>([]);
  const [catalogs, setCatalogs] = useState<string[]>([]);
  const [catalogMetadata, setCatalogMetadata] = useState<{
    [catalog: string]: CatalogMetadata | null;
  }>({});

  const [catalogTagText, setCatalogTagText] = useState("Catalogs");
  const [catalogTagIcon, setCatalogTagIcon] = useState(faFilm);

  const [catalogIconState, setCatalogIconState] = useState<{
    [catalog: string]: IconDefinition;
  }>({});

  async function fetchApiMetadata() {
    let data: ApiMetadata;
    try {
      data = await getApiMetadata();
    } catch (error) {
      setCatalogTagText("Error fetching API metadata");
      setCatalogTagIcon(faCircleExclamation);
      return;
    }
    const endpoints = Object.keys(data.paths).filter((path) =>
      path.toLowerCase().startsWith("/stremio/catalog/")
    );
    setEndpoints(endpoints);
    const cats = endpoints.map((path) => path.split("/")[4].split(".")[0]);
    setCatalogs(cats);
  }

  async function fetchCatalogImages() {
    const initialIcons: { [catalog: string]: IconDefinition } = {};
    endpoints.forEach((endpoint) => {
      const catalog = endpoint.split("/")[4]?.split(".")[0];
      initialIcons[catalog] = faSpinner;
    });
    setCatalogIconState(initialIcons);

    const tasks = endpoints.map(async (endpoint) => {
      let requestUrl = settings.mediaApiUrl + endpoint.substring(1);
      if (endpoint.includes("{") && endpoint.includes("}"))
        requestUrl = requestUrl.replace(/{[^}]+}/g, "all");
      const catalog = endpoint.split("/")[4]?.split(".")[0];
      try {
        const response = await fetch(requestUrl);
        const data: CatalogMetadata | null = await response.json();
        setCatalogIconState((prev) => ({
          ...prev,
          [catalog]:
            data && data.metas && data.metas.length > 0 ? faCheck : faTimes,
        }));
        setCatalogMetadata((prev) => ({ ...prev, [catalog]: data }));
      } catch (error) {
        setCatalogIconState((prev) => ({
          ...prev,
          [catalog]: faTimes,
        }));
        setCatalogMetadata((prev) => ({ ...prev, [catalog]: null }));
      }
    });
    await Promise.all(tasks);
  }

  useEffect(() => {
    fetchApiMetadata();
  }, []);

  useEffect(() => {
    fetchCatalogImages();
  }, [endpoints]);

  function visitImdbPage(imdbId: string) {
    const imdbUrl = `${settings.imdbUrl}title/${imdbId}/`;
    window.open(imdbUrl, "_blank");
  }

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
                <FontAwesomeIcon
                  icon={faCopy}
                  className="stremio-manifest-copy"
                  onClick={() => {
                    navigator.clipboard.writeText(
                      settings.mediaApiUrl + "stremio/manifest.json"
                    );
                  }}
                />
              </div>
            </div>
          }
        ></ExpandPanel>
      </div>
      <div className="stremio-section">
        <h2 className="stremio-tag">
          {catalogTagText}
          <FontAwesomeIcon
            className="tag-icon"
            icon={catalogTagIcon}
          ></FontAwesomeIcon>
        </h2>
        {catalogs.map((catalog) => (
          <ExpandPanel
            key={catalog}
            titleChildren={
              <div className="stremio-panel-title">
                {catalog.charAt(0).toUpperCase() + catalog.slice(1)}
                <FontAwesomeIcon
                  className={`panel-icon loading-icon${
                    (catalogIconState[catalog] || faSpinner) === faSpinner
                      ? " loading"
                      : ""
                  }`}
                  style={{
                    color:
                      (catalogIconState[catalog] === faCheck &&
                        "rgb(44, 192, 101)") ||
                      (catalogIconState[catalog] === faTimes &&
                        "rgba(202, 55, 55, 1)") ||
                      "white",
                  }}
                  icon={catalogIconState[catalog] || faSpinner}
                />
              </div>
            }
            expandChildren={
              <div>
                <p className="stremio-catalog-count">
                  {catalogMetadata[catalog]?.metas
                    ? `${catalogMetadata[catalog].metas.length} media result${
                        catalogMetadata[catalog].metas.length === 1 ? "" : "s"
                      }`
                    : "Failed to fetch media results"}
                </p>
                {catalogMetadata[catalog] ? (
                  <div className="stremio-catalog-images">
                    {catalogMetadata[catalog]?.metas?.map((meta) =>
                      meta.poster ? (
                        <img
                          key={meta.id}
                          src={meta.poster}
                          alt={meta.name}
                          onClick={() => visitImdbPage(meta.id)}
                        />
                      ) : null
                    )}
                  </div>
                ) : null}
              </div>
            }
          />
        ))}
      </div>
    </div>
  );
}
