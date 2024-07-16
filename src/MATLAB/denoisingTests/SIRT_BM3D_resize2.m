

nProjections = length(Projections);

angleStep=360.0/nProjections;

sizeProjections = length(Projections{1});


reconImageSIRT=imresize( reconImage,2);


s=size(reconImageSIRT);
R_Thresh = (s(1)/2.1)^2;
centerI= s(1)/2;
centerJ= s(2)/2;

mask=zeros(s);

for I=1:s(1)
        for J=1:s(2)
            R= ((I-centerI)^2 + (J-centerJ)^2)/R_Thresh;
            if R>1
                mask(I,J)=1;
            end
        end
end
maskIndex = find(mask==1);


clear forward;
for K=1:5
    
    reconImageSIRT(maskIndex)=0;
    
    diffImage = zeros(s);
    
    for I=1:nProjections
        projection = Projections{I};

        tempImage = imrotate(reconImageSIRT,-1*angleStep*I,'bilinear','crop'); 
        tempImage = imresize(tempImage,.5);
        
        forward = sum(tempImage);
        diffProjections{I}=projection -forward;
    end

    
    addition = zeros([sizeProjections sizeProjections]);
    
    for I=1:nProjections
        diff = diffProjections{I}/(sizeProjections^2);
     
        for J=1:sizeProjections
           addition(:,J)= diff(J);
        end

        addition=imrotate(addition,angleStep*I,'bilinear','crop');
        additionT=imresize(addition,2);
        diffImage=diffImage + additionT;
    end
  
    imagesc(diffImage);
    drawnow;

    regularization = Poisson_denoising_Anscombe_exact_unbiased_inverse(reconImageSIRT) ;
    reconImageSIRT=(reconImageSIRT + .3* regularization)./1.3 + diffImage;
end


R_Thresh = (sizeProjections/2.2)^2;
sumV=0;
cc=0;
for I=1:sizeProjections
        for J=1:sizeProjections
            R= ((I-centerI)^2 + (J-centerJ)^2)/R_Thresh;
            if R>1 && mask(I,J)~=1
                sumV=sumV + reconImageSIRT(I,J);
                cc=cc+1;
            end
        end
end

average = sumV/cc;

reconImageSIRT(maskIndex)=average;


figure;imagesc(reconImageSIRT);colormap gray;
figure;imagesc(D);colormap gray;


double(max(D(:)))/max(reconImageSIRT(:))