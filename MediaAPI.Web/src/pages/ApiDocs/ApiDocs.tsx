import { useEffect, useRef, useState } from "react";
import Header from "../../assets/Header/Header";
import { getApiMetadata, type ApiMetadata } from "../../utils/ApiMetadata";
import "./ApiDocs.css";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faTag, faTimes } from "@fortawesome/free-solid-svg-icons";
import Settings from "../../Settings";
import ExpandPanel from "../../assets/ExpandPanel/ExpandPanel";
import "../../assets/ExpandPanel/ExpandPanel.css";

export default function ApiDocs() {
  const settings = new Settings();

  const [apiMetadata, setApiMetadata] = useState<ApiMetadata | null>(null);
  const [tags, setTags] = useState<string[]>([]);
  const [endpoints, setEndpoints] = useState<string[]>([]);

  const [endpointState, setEndpointState] = useState<{
    [endpoint: string]: string | null;
  }>({});

  const paramValueRefs = useRef<{
    [endpoint: string]: {
      [paramName: string]: HTMLInputElement | null;
    };
  }>({});

  async function fetchApiMetadata() {
    let metadata: ApiMetadata;
    try {
      metadata = await getApiMetadata();
    } catch (e) {
      setTags(["Error fetching API metadata"]);
      return;
    }
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
    setEndpointState(
      Object.keys(metadata.paths).reduce((acc, endpoint) => {
        acc[endpoint] = settings.mediaApiUrl + endpoint.substring(1);
        return acc;
      }, {} as { [endpoint: string]: string })
    );
  }

  useEffect(() => {
    fetchApiMetadata();
  }, []);

  function updateEndpointState(endpoint: string) {
    let ep = settings.mediaApiUrl + endpoint.substring(1);
    for (let pName in paramValueRefs.current[endpoint]) {
      let inputRef = paramValueRefs.current[endpoint][pName];
      if (inputRef) {
        ep = ep.replace(`{${pName}}`, inputRef.value || `{${pName}}`);
      }
    }
    setEndpointState((prev) => ({
      ...prev,
      [endpoint]: ep,
    }));
  }

  async function executeEndpoint(endpoint: string) {
    const newWindow = window.open();
    if (newWindow) {
      newWindow.document.title = "media-api Response";
      newWindow.document.body.style.background = "rgb(31, 31, 31)";
      newWindow.document.body.style.color = "rgba(255, 255, 255)";
      newWindow.document.body.style.fontFamily = "monospace";
      newWindow.document.body.innerHTML = `<pre>Fetching response...</pre>`;
      const iconSvg = await fetch("/code.svg").then((res) => res.text());
      const link = newWindow.document.createElement("link");
      link.rel = "icon";
      link.href = `data:image/svg+xml,${encodeURIComponent(iconSvg)}`;
      newWindow.document.head.appendChild(link);
    }

    let params = paramValueRefs.current[endpoint];
    for (let paramName in params) {
      let input = params[paramName];
      endpoint = endpoint.replace(`{${paramName}}`, input?.value || "");
    }

    let data: any;
    try {
      let response = await fetch(settings.mediaApiUrl + endpoint.substring(1));
      data = await response.json();
    } catch (error) {
      data = { error: "Failed to fetch endpoint." };
    }

    if (newWindow) {
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
                <ExpandPanel
                  key={endpoint}
                  titleChildren={
                    <div>
                      <div className="api-url">
                        <p className="api-method">
                          {Object.keys(apiMetadata?.paths?.[endpoint] || {})}
                        </p>
                        <p>{endpoint}</p>
                      </div>
                      <p className="api-summary">
                        {apiMetadata?.paths?.[endpoint]?.get?.summary || ""}
                      </p>
                    </div>
                  }
                  expandChildren={
                    <div className="api-parameters">
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
                            <div className="param-input-panel">
                              <input
                                className="param-value"
                                placeholder={param.name + " value"}
                                onChange={() => updateEndpointState(endpoint)}
                                ref={(el) => {
                                  if (!paramValueRefs.current[endpoint]) {
                                    paramValueRefs.current[endpoint] = {};
                                  }
                                  paramValueRefs.current[endpoint][param.name] =
                                    el;
                                }}
                              ></input>
                              <button
                                className="param-clear-button"
                                onClick={() => {
                                  const inputRef =
                                    paramValueRefs.current[endpoint]?.[
                                      param.name
                                    ];
                                  if (inputRef) {
                                    inputRef.value = "";
                                  }
                                  updateEndpointState(endpoint);
                                }}
                              >
                                <FontAwesomeIcon icon={faTimes} />
                              </button>
                            </div>
                          </div>
                        )
                      )}
                      <div className="execute-panel">
                        <button
                          className="execute-button"
                          onClick={() => executeEndpoint(endpoint)}
                        >
                          Execute
                        </button>
                        <p className="endpoint-state">
                          {endpointState[endpoint]}
                        </p>
                      </div>
                    </div>
                  }
                ></ExpandPanel>
              ))}
          </div>
        ))}
      </div>
    </div>
  );
}
