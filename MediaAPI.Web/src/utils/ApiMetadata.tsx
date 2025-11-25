import Settings from "../Settings";

export interface ApiMetadata {
  paths: {
    [endpoint: string]: {
      get?: {
        summary?: string;
        parameters?: Array<{
          name: string;
          description?: string;
          in: string;
          required: boolean;
          schema: {
            type: string;
          };
        }>;
      };
      post?: {
        summary?: string;
        parameters?: Array<{
          name: string;
          description?: string;
          in: string;
          required: boolean;
          schema: {
            type: string;
          };
        }>;
      };
      put?: {
        summary?: string;
        parameters?: Array<{
          name: string;
          description?: string;
          in: string;
          required: boolean;
          schema: {
            type: string;
          };
        }>;
      };
      delete?: {
        summary?: string;
        parameters?: Array<{
          name: string;
          description?: string;
          in: string;
          required: boolean;
          schema: {
            type: string;
          };
        }>;
      };
    };
  };
}

export async function getApiMetadata() {
  const settings = new Settings();
  const response = await fetch(settings.swaggerUrl);
  const data = await response.json();
  return data;
}
