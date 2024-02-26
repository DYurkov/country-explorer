// App.tsx

import React from "react";
import { BrowserRouter as Router, Route, Routes } from "react-router-dom";
import CountryTable from "./components/CountryTable";
import CountryDetails from "./components/CountryDetails";

function App() {
  return (
    <Router>
      <div className="App">
        <Routes>
          <Route path="/" element={<CountryTable />} />
          <Route path="/countries/:name" element={<CountryDetails />} />
        </Routes>
      </div>
    </Router>
  );
}

export default App;
