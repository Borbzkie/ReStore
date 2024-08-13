import { Product } from "../../app/models/product";
import ProductList from "./ProductList";
import { useEffect, useState } from "react";


export default function Catalog () {
    const [products, setproducts] = useState<Product[]>([]);

    useEffect(() =>{
        fetch('http://localhost:5000/api/Products')
        .then(response => 
        response.json())
        .then(data => setproducts(data))
    },[])

    return (
        <>
            <ProductList products={products} ></ProductList>
        </>
    )
}