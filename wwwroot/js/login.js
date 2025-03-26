const uri = '/User/Login';
let user = [];

const toSend = (event) => {
    event.preventDefault();
    let name = document.getElementById('name');
    let passWord = document.getElementById('passWord');
    login(name.value, passWord.value);
}

const login = async (name, passWord) => {
    user = {
        "UserId": 0, 
        "Username": name,
        "Password": passWord,
        "Email":" ",
        "isAdmin": false
    };
    try {
        const response = await fetch(uri, {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                "Content-Type": "application/json",

            },
            body: JSON.stringify(user)
        });
       const responseText = await response.text(); 
        if (!response.ok) { 
            alert("Incorrect username or password!");
            return;
        }

       const cleanToken = responseText.replace(/"/g, ''); 

        if (!cleanToken) { 
            alert("Incorrect username or password!");
            return;
        } 
        sessionStorage.setItem('token', cleanToken); 
        let token= sessionStorage.getItem('token');
        let payload = JSON.parse(atob(token.split('.')[1])); 
        const isAdmin = payload.isAdmin; 
        if(isAdmin==='true')
            location.href="admin.html";
        else
            location.href = "book.html"; 

    } catch (error) {
        console.error('Login error', error);
        alert("Login error, please try again later.");
    }
}
window.onload = function () {

    google.accounts.id.initialize({
        client_id: "615496630298-8lb43opj1vhse725dql9aucvh1369th0.apps.googleusercontent.com",
        callback: handleCredentialResponse
    });
    google.accounts.id.renderButton(
        document.getElementById("g_id_signin"),
        { theme: "outline", size: "large" }
    );
    
};

function handleCredentialResponse(response) {
    
    fetch("https://localhost:5074/account/google-login", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ token: response.credential })
    })
    .then(res => res.json())
    .then(data => {
        console.log("Received token:", data.token);
        if (!data.token) {
            throw new Error("No token received from server");
        }
        sessionStorage.setItem("token", data.token);

        let payload = JSON.parse(atob(data.token.split('.')[1]));
        const isAdmin = payload.isAdmin;        
        if (isAdmin === "true") {
            location.href = "admin.html";
        } else {
            location.href = "book.html";
        }
    })
    .catch(error => console.error("Error:", error));
}
