import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faFilm,
  faTv,
  faClapperboard,
  faCompactDisc,
  faTicket,
  faVideo,
} from "@fortawesome/free-solid-svg-icons";
import "./IconRain.css";

const icons = [faFilm, faTv, faClapperboard, faCompactDisc, faTicket, faVideo];

export default function IconRain() {
  const numberOfIcons = 30;

  return (
    <div className="icon-rain">
      {[...Array(numberOfIcons)].map((_, i) => (
        <FontAwesomeIcon
          key={i}
          icon={icons[Math.floor(Math.random() * icons.length)]}
          className="rain-icon"
          style={{
            left: `${Math.random() * 100}%`,
            animationDelay: `${Math.random() * 3}s`,
          }}
        />
      ))}
    </div>
  );
}
