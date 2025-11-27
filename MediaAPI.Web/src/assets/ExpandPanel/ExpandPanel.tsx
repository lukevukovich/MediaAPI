import { faAngleDown } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { useEffect, useRef } from "react";
import "./ExpandPanel.css";

type ExpandPanelProps = {
  titleChildren?: React.ReactNode;
  expandChildren?: React.ReactNode;
};

export default function ExpandPanel({
  titleChildren,
  expandChildren,
}: ExpandPanelProps) {
  const expandPanelRef = useRef<HTMLDivElement>(null);
  const expandIconRef = useRef<SVGSVGElement>(null);

  function rotateExpandIcon() {
    const iconRef = expandIconRef.current;
    if (iconRef) {
      iconRef.classList.toggle("expanded");
    }
  }

  function expand() {
    const ref = expandPanelRef.current;
    if (ref) {
      if (ref.classList.contains("expanded")) {
        ref.style.maxHeight = "0px";
        ref.classList.remove("expanded");
      } else {
        ref.style.maxHeight = ref.scrollHeight + "px";
        ref.classList.add("expanded");
      }
    }
    rotateExpandIcon();
  }

  function resize() {
    const ref = expandPanelRef.current;
    if (ref && ref.classList.contains("expanded"))
      ref.style.maxHeight = ref.scrollHeight + "px";
  }

  useEffect(() => {
    let resizeTimeout: number | undefined;
    function handleResize() {
      if (resizeTimeout) {
        clearTimeout(resizeTimeout);
      }
      resizeTimeout = window.setTimeout(() => {
        resize();
      }, 100);
    }
    window.addEventListener("resize", handleResize);
    return () => {
      window.removeEventListener("resize", handleResize);
      if (resizeTimeout) {
        clearTimeout(resizeTimeout);
      }
    };
  }, []);

  return (
    <div>
      <h3 className="panel">
        <div className="panel-heading">
          <div className="panel-title">{titleChildren}</div>
          <button className="button-expand" onClick={() => expand()}>
            <FontAwesomeIcon
              className="icon-expand"
              icon={faAngleDown}
              ref={expandIconRef}
            />
          </button>
        </div>
        <div className="panel-expand" ref={expandPanelRef}>
          {expandChildren}
        </div>
      </h3>
    </div>
  );
}
