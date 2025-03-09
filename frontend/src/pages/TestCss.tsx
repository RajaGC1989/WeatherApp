import { Box } from "@mui/material";

export default function TestCss() {
  return (
    <Box
      justifyContent="center"
      alignItems="center"
      display={"flex"}
      height="100vh" // Takes full viewport height
      width="100vw" // Ensures full width
      bgcolor="#f4f4f4" // Optional background color
    >
      <Box
        sx={{
          border: "1px solid red",
          padding: "40px",
          justifyContent: "center",
          alignItems: "center",
          display: "flex",
          bgcolor: "white",
        }}
      >
        This is a Box component
      </Box>
    </Box>
  );
}
