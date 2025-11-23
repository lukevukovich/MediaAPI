import { useEffect, useRef, useState } from "react";
import Header from "../../assets/Header/Header";
import { getApiMetadata, type ApiMetadata } from "../../utils/ApiMetadata";
import "./Documentation.css";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faAngleDown, faTag } from "@fortawesome/free-solid-svg-icons";
import Settings from "../../utils/Settings";

export default function Documentation() {
  const settings = new Settings();
  const [apiMetadata, setApiMetadata] = useState<ApiMetadata | null>(null);
  const [tags, setTags] = useState<string[]>([]);
  const [endpoints, setEndpoints] = useState<string[]>([]);
  const expandEndpointRefs = useRef<{
    [endpoint: string]: HTMLDivElement | null;
  }>({});
  const expandIconRefs = useRef<{
    [endpoint: string]: SVGSVGElement | null;
  }>({});

  async function fetchApiMetadata() {
    const metadata = await getApiMetadata();
    setApiMetadata(metadata);
    setTags(
      Array.from(
        new Set(
          Object.keys(metadata.paths)
            .map((path) => path.split("/")[1])
            .filter(Boolean)
        )
      )
    );
    setEndpoints(Object.keys(metadata.paths));
  }

  useEffect(() => {
    fetchApiMetadata();
  }, []);

  function rotateExpandIcon(endpoint: string) {
    const iconRef = expandIconRefs.current[endpoint];
    if (iconRef) {
      iconRef.classList.toggle("expanded");
    }
  }

  function expandEndpoint(endpoint: string) {
    const ref = expandEndpointRefs.current[endpoint];
    if (ref) {
      if (ref.classList.contains("expanded")) {
        ref.style.maxHeight = "0px";
        ref.classList.remove("expanded");
      } else {
        ref.style.maxHeight = ref.scrollHeight + "px";
        ref.classList.add("expanded");
      }
    }
    rotateExpandIcon(endpoint);
  }

  async function executeEndpoint(endpoint: string) {
    let response = await fetch(settings.mediaApiUrl + endpoint.substring(1));
    let data = await response.json();
    // open a new window and display the JSON data
    const newWindow = window.open();
    if (newWindow) {
      newWindow.document.title = "media-api Result";
      newWindow.document.body.style.background = "rgb(31, 31, 31)";
      newWindow.document.body.style.color = "rgba(255, 255, 255)";
      newWindow.document.body.style.fontFamily = "monospace";
      newWindow.document.body.innerHTML = `<pre>${JSON.stringify(
        data,
        null,
        2
      )}</pre>`;
    }
  }

  return (
    <div className="docs-page">
      <Header />
      <div className="api-metadata">
        {tags.map((tag) => (
          <div className="api-section" key={tag}>
            <h2 className="api-tag">
              {tag}
              <FontAwesomeIcon icon={faTag} className="tag-icon" />
            </h2>
            {endpoints
              .filter((endpoint) => endpoint.startsWith(`/${tag}/`))
              .map((endpoint) => (
                <h3 className="api-endpoint" key={endpoint}>
                  <div className="api-endpoint-left">
                    <div className="api-url">
                      <p className="api-method">
                        {Object.keys(apiMetadata?.paths?.[endpoint] || {})}
                      </p>
                      <p>{endpoint}</p>
                    </div>
                    <p className="api-summary">
                      {apiMetadata?.paths?.[endpoint]?.get?.summary ||
                        apiMetadata?.paths?.[endpoint]?.post?.summary ||
                        apiMetadata?.paths?.[endpoint]?.put?.summary ||
                        apiMetadata?.paths?.[endpoint]?.delete?.summary ||
                        ""}
                    </p>
                    <div
                      className="api-parameters"
                      ref={(el) => {
                        expandEndpointRefs.current[endpoint] = el;
                      }}
                    >
                      {apiMetadata?.paths?.[endpoint]?.get?.parameters?.map(
                        (param) => (
                          <div key={param.name} className="api-parameter">
                            <p className="param-name">{param.name}</p>
                            <p className="param-description">
                              {param.description
                                ?.replace(/- /g, ", ")
                                .replace(/\s*,\s*/g, ", ")
                                .replace(/^, /, "")
                                .replace(/ +/g, " ")
                                .replace(":,", ":")
                                .trim()}
                            </p>
                            <p className="param-type">{param.schema.type}</p>
                            <p className="param-required">
                              {param.required ? "required" : "optional"}
                            </p>
                          </div>
                        )
                      )}
                      <button
                        className="execute-button"
                        onClick={() => executeEndpoint(endpoint)}
                      >
                        Execute
                      </button>
                    </div>
                  </div>
                  <div className="api-endpoint-right">
                    <button
                      className="expand-button"
                      onClick={() => expandEndpoint(endpoint)}
                    >
                      <FontAwesomeIcon
                        className="expand-icon"
                        icon={faAngleDown}
                        ref={(el) => {
                          expandIconRefs.current[endpoint] = el;
                        }}
                      />
                    </button>
                  </div>
                </h3>
              ))}
          </div>
        ))}
      </div>
    </div>
  );
}
