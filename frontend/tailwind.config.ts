import type { Config } from "tailwindcss";

const config: Config = {
  content: [
    "./src/pages/**/*.{js,ts,jsx,tsx,mdx}",
    "./src/components/**/*.{js,ts,jsx,tsx,mdx}",
    "./src/app/**/*.{js,ts,jsx,tsx,mdx}",
  ],
  theme: {
    extend: {
      colors: {
        canvas: "#FBFBF9",
        obsidian: "#0F0F10",
        terracotta: {
          DEFAULT: "#C2410C",
          hover: "#9A3412",
        },
        ink: {
          primary: "#0F0F10",
          secondary: "#52525B",
          muted: "#71717A",
        },
        frame: {
          subtle: "#E4E4E7",
          divider: "#D4D4D8",
          dark: "#27272A",
        },
      },
      fontFamily: {
        sans: ["system-ui", "-apple-system", "BlinkMacSystemFont", "Segoe UI", "Roboto", "sans-serif"],
        mono: ["ui-monospace", "SFMono-Regular", "Menlo", "Monaco", "Consolas", "monospace"],
      },
    },
  },
  plugins: [],
};
export default config;
