import { useEffect, useState } from "react";
import Header from "../../assets/Header/Header";
import { getApiMetadata, type ApiMetadata } from "../../utils/ApiMetadata";
import "./Documentation.css";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faAngleDown, faTag } from "@fortawesome/free-solid-svg-icons";

export default function Documentation() {
  const [apiMetadata, setApiMetadata] = useState<ApiMetadata | null>(null);
  const [tags, setTags] = useState<string[]>([]);
  const [endpoints, setEndpoints] = useState<string[]>([]);

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
                  </div>
                  <div className="api-endpoint-right">
                    <button className="expand-button">
                      <FontAwesomeIcon icon={faAngleDown} />
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
