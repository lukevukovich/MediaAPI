import Settings from "./Settings";

export interface ApiMetadata {
  paths: {
    [endpoint: string]: {
      get?: {
        summary?: string;
      };
      post?: {
        summary?: string;
      };
      put?: {
        summary?: string;
      };
      delete?: {
        summary?: string;
      };
    };
  };
}

export async function getApiMetadata() {
  const settings = new Settings();
  const response = await fetch(settings.swaggerUrl);
  const data = await response.json();
  console.log(data);
  return data;
}
