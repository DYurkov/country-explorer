import React, { useEffect, useState } from "react";
import { Link, useLocation, useNavigate } from "react-router-dom";
import { CountrySummary } from "../models/Country";
import { fetchAllCountries } from "../services/CountryService";
import Pagination from "./Pagination";

const PAGE_SIZE = 20;

const CountryTable: React.FC = () => {
  const [countries, setCountries] = useState<CountrySummary[]>([]);
  const [filteredCountries, setFilteredCountries] = useState<CountrySummary[]>(
    []
  );
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(0);
  const [searchTerm, setSearchTerm] = useState("");

  const navigate = useNavigate();
  const location = useLocation();

  useEffect(() => {
    const query = new URLSearchParams(location.search);
    const page = parseInt(query.get("page") || "1", 10);
    if (page !== currentPage) {
      setCurrentPage(page);
    }
  }, [location.search]);

  useEffect(() => {
    const getCountries = async () => {
      const data = await fetchAllCountries();
      setCountries(data);
      setFilteredCountries(data);
      setTotalPages(Math.ceil(data.length / PAGE_SIZE));
    };

    getCountries();
  }, []);

  useEffect(() => {
    const filtered = countries.filter((country) =>
      country.name.common.toLowerCase().includes(searchTerm.toLowerCase())
    );
    setFilteredCountries(filtered);
    setTotalPages(Math.ceil(filtered.length / PAGE_SIZE));
  }, [searchTerm, countries]);

  const onPageChange = (page: number) => {
    setCurrentPage(page);
    navigate(`?page=${page}`);
  };

  const displayedCountries = filteredCountries.slice(
    (currentPage - 1) * PAGE_SIZE,
    currentPage * PAGE_SIZE
  );

  return (
    <div className="container mt-5">
      <div className="mb-3">
        <input
          type="text"
          className="form-control"
          placeholder="Search by country name..."
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
        />
      </div>
      {displayedCountries?.length ? (
        <table className="table">
          <thead>
            <tr>
              <th>Name</th>
              <th>Capital</th>
              <th>Currency</th>
              <th>Language</th>
              <th>Region</th>
            </tr>
          </thead>
          <tbody>
            {displayedCountries.map((country, index) => (
              <tr key={index}>
                <td>
                  <Link to={`/countries/${country.name.common}`}>
                    {country.name.common}
                  </Link>
                </td>
                <td>{country.capital?.join(", ") || "-"}</td>
                <td>
                  {country.currencies
                    ? Object.keys(country.currencies).map((curr, idx) => (
                        <span key={idx}>
                          {country.currencies[curr].name} (
                          {country.currencies[curr].symbol})
                          {idx !== Object.keys(country.currencies).length - 1 &&
                            ", "}
                        </span>
                      ))
                    : "-"}
                </td>
                <td>
                  {country.languages
                    ? Object.values(country.languages).join(", ")
                    : "-"}
                </td>
                <td>
                  {country.region}{" "}
                  {country.subregion ? `(${country.subregion})` : ""}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      ) : (
        <div className="container mt-5">Loading...</div>
      )}
      <Pagination
        totalPages={totalPages}
        currentPage={currentPage}
        onPageChange={onPageChange}
      />
    </div>
  );
};

export default CountryTable;
